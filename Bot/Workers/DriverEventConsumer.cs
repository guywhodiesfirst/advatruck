namespace Bot.Workers;

using System.Text;
using System.Text.Json;
using Bot.Interfaces;
using Bot.UI;
using Data.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

public class DriverEventConsumer(
    IConnection connection,
    ITelegramBotClient bot,
    IDriverSessionStore sessions,
    ITrackingService tracking,
    ILogger<DriverEventConsumer> logger)
    : BackgroundService
{
    private const string ExchangeName = "tms.driver.events";
    private const string QueueName = "bot.notification.queue";
    private IModel? _channel;

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            _channel = connection.CreateModel();

            _channel.ExchangeDeclare(ExchangeName, ExchangeType.Topic, durable: true);
            _channel.QueueDeclare(QueueName, durable: true, exclusive: false, autoDelete: false);

            _channel.QueueBind(QueueName, ExchangeName, "driver.inactive");
            _channel.QueueBind(QueueName, ExchangeName, "load.assigned");
            _channel.QueueBind(QueueName, ExchangeName, "load.deassigned");
            _channel.QueueBind(QueueName, ExchangeName, "load.canceled");
            _channel.QueueBind(QueueName, ExchangeName, "load.ongoing");

            _channel.BasicQos(0, 10, false);

            logger.LogInformation("Bot Messaging Consumer started. Listening on: {Queue}", QueueName);
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "Could not initialize RabbitMQ connection in Bot service");
        }

        return base.StartAsync(cancellationToken);
    }

    protected override Task ExecuteAsync(CancellationToken cancellationToken)
    {
        if (_channel == null)
        {
            return Task.CompletedTask;
        }

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (_, ea) =>
        {
            try
            {
                var body = ea.Body.ToArray();
                var json = Encoding.UTF8.GetString(body);

                var integrationEvent = JsonSerializer.Deserialize<DriverIntegrationEvent>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                });

                if (integrationEvent != null)
                {
                    await HandleEventAsync(integrationEvent, cancellationToken);
                }

                _channel.BasicAck(ea.DeliveryTag, false);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to process message from {RoutingKey}", ea.RoutingKey);
                _channel.BasicNack(ea.DeliveryTag, false, false);
            }
        };

        _channel.BasicConsume(QueueName, autoAck: false, consumer: consumer);
        return Task.CompletedTask;
    }

    private async Task HandleEventAsync(DriverIntegrationEvent @event, CancellationToken ct)
    {
        var chatId = await sessions.GetChatIdByDriverIdAsync(@event.DriverId);
        if (chatId == null)
        {
            return;
        }

        string shortId = @event.LoadId?.ToString().Split('-')[0] ?? "???";

        switch (@event.EventType)
        {
            case "load.assigned":
                await SendLoadAssignmentAsync(chatId.Value, @event.LoadId, shortId, ct);
                break;

            case "load.deassigned":
                await SendLoadDeassignmentAsync(chatId.Value, shortId, ct);
                break;

            case "load.ongoing":
                await SendTripStartedAsync(chatId.Value, @event.LoadId, shortId, ct);
                break;

            case "load.canceled":
                await SendLoadCancellationAsync(chatId.Value, shortId, ct);
                break;

            case "driver.inactive":
                await HandleDriverInactivityAsync(chatId.Value, @event.DriverId, ct);
                break;
        }
    }

    private async Task HandleDriverInactivityAsync(long chatId, Guid driverId, CancellationToken ct)
    {
        var isTracking = await sessions.IsTrackingActiveAsync(driverId);

        if (isTracking)
        {
            var lastStop = await sessions.GetTrackingStoppedAtAsync(driverId);
            if (lastStop != null && DateTime.UtcNow - lastStop < TimeSpan.FromMinutes(2))
            {
                return;
            }

            await tracking.StopTrackingAsync(chatId);
        }

        const string text = "⚠️ *Геолокація не активна*\n\n" +
                            "Ваш рейс вже триває, але ми не отримуємо дані про місцезнаходження\\. " +
                            "Будь ласка, перевірте зв'язок та переконайтеся, що відстеження увімкнено\\.";

        await bot.SendMessage(
            chatId: chatId,
            text: text,
            parseMode: ParseMode.MarkdownV2,
            cancellationToken: ct);
    }

    private async Task SendLoadAssignmentAsync(long chatId, Guid? loadId, string shortId, CancellationToken ct)
    {
        string text = $"📦 *Призначено новий вантаж: #{shortId}*\n\n" +
                      "Диспетчер додав вам новий рейс\\. Натисніть кнопку нижче, щоб переглянути деталі\\.";

        var keyboard = KeyboardLayout.LoadDetailsKeyboard(loadId);

        await bot.SendMessage(chatId, text, parseMode: ParseMode.MarkdownV2, replyMarkup: keyboard, cancellationToken: ct);
    }

    private async Task SendLoadDeassignmentAsync(long chatId, string shortId, CancellationToken ct)
    {
        string text = $"🔄 *Зміна планів: #{shortId}*\n\n" +
                      "Вас було знято з виконання цього рейсу диспетчером\\. Вантаж більше не закріплений за вами\\.";

        await bot.SendMessage(chatId, text, parseMode: ParseMode.MarkdownV2, cancellationToken: ct);
    }

    private async Task SendTripStartedAsync(long chatId, Guid? loadId, string shortId, CancellationToken ct)
    {
        string text = $"🚀 *Час вирушати: #{shortId}*\n\n" +
                      "Ваш запланований рейс тепер активний\\. Відкрийте деталі, щоб розпочати навігацію та звітність\\.";

        var keyboard = KeyboardLayout.LoadDetailsKeyboard(loadId);

        await bot.SendMessage(chatId, text, parseMode: ParseMode.MarkdownV2, replyMarkup: keyboard, cancellationToken: ct);
    }

    private async Task SendLoadCancellationAsync(long chatId, string shortId, CancellationToken ct)
    {
        string text = $"❌ *Вантаж скасовано: #{shortId}*\n\n" +
                      "Цей рейс було скасовано диспетчером\\. Будь ласка, очікуйте на нові замовлення\\.";

        await bot.SendMessage(chatId, text, parseMode: ParseMode.MarkdownV2, cancellationToken: ct);
    }

    public override void Dispose()
    {
        _channel?.Close();
        base.Dispose();
    }

    private class DriverIntegrationEvent
    {
        public Guid DriverId { get; set; }

        public Guid? LoadId { get; set; }

        public string EventType { get; set; } = string.Empty;

        public DateTime Timestamp { get; set; }
    }
}
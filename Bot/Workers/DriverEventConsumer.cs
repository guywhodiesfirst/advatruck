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

/// <summary>
/// Orchestrates Telegram notifications based on RabbitMQ events.
/// </summary>
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

            // Infrastructure setup: Ensure exchange and queue exist
            _channel.ExchangeDeclare(ExchangeName, ExchangeType.Topic, durable: true);
            _channel.QueueDeclare(QueueName, durable: true, exclusive: false, autoDelete: false);

            _channel.QueueBind(QueueName, ExchangeName, "driver.inactive");
            _channel.QueueBind(QueueName, ExchangeName, "load.assigned");
            _channel.QueueBind(QueueName, ExchangeName, "load.canceled");

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
                _channel.BasicNack(ea.DeliveryTag, false, true);
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

        switch (@event.EventType)
        {
            case "load.assigned":
                await SendLoadAssignmentAsync(chatId.Value, @event.LoadId, ct);
                break;

            case "load.canceled":
                await SendLoadCancellationAsync(chatId.Value, ct);
                break;

            case "driver.inactive":
                await HandleDriverInactivityAsync(chatId.Value, @event.DriverId, ct);
                break;
        }
    }

    private async Task HandleDriverInactivityAsync(long chatId, Guid driverId, CancellationToken ct)
    {
        var isTracking = await sessions.IsTrackingActiveAsync(driverId);
        if (!isTracking)
        {
            return;
        }

        var lastStop = await sessions.GetTrackingStoppedAtAsync(driverId);
        if (lastStop != null && DateTime.UtcNow - lastStop < TimeSpan.FromMinutes(2))
        {
            return;
        }

        await tracking.StopTrackingAsync(chatId);

        const string text = "⚠️ *Втрачено сигнал GPS*\n\n" +
                            "Ми припинили відстеження, оскільки дані не надходять\\. Будь ласка, та увімкніть його знову\\.";

        await bot.SendMessage(
            chatId: chatId,
            text: text,
            parseMode: ParseMode.MarkdownV2,
            cancellationToken: ct);

        logger.LogInformation("Inactivity notification processed for driver {DriverId}", driverId);
    }

    private async Task SendLoadAssignmentAsync(long chatId, Guid? loadId, CancellationToken ct)
    {
        const string text = "📦 *Нове замовлення призначено\\!*\n\n" +
                            "Диспетчер додав вам новий рейс\\. Натисніть кнопку нижче, щоб переглянути деталі\\.";

        var keyboard = KeyboardLayout.LoadDetailsKeyboard(loadId);

        await bot.SendMessage(
            chatId: chatId,
            text: text,
            parseMode: ParseMode.MarkdownV2,
            replyMarkup: keyboard,
            cancellationToken: ct);
    }

    private async Task SendLoadCancellationAsync(long chatId, CancellationToken ct)
    {
        const string text = "❌ *Вантаж скасовано*\n\n" +
                            "Поточний рейс було скасовано диспетчером\\. Очікуйте на нові замовлення\\.";

        await bot.SendMessage(
            chatId: chatId,
            text: text,
            parseMode: ParseMode.MarkdownV2,
            cancellationToken: ct);
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
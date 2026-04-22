namespace Bot.Workers;

using System.Text;
using System.Text.Json;
using Bot.Interfaces;
using Data.Interfaces;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Telegram.Bot;

public class DriverEventConsumer(
    IConnection connection,
    ITrackingService tracking,
    ITelegramBotClient bot,
    IDriverSessionStore sessions)
    : BackgroundService
{
    private IModel? _channel;

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        _channel = connection.CreateModel();

        _channel.QueueDeclare(
            queue: "driver.inactive",
            durable: true,
            exclusive: false,
            autoDelete: false);

        _channel.BasicQos(0, 10, false);

        return base.StartAsync(cancellationToken);
    }

    protected override Task ExecuteAsync(CancellationToken cancellationToken)
    {
        var consumer = new EventingBasicConsumer(_channel);

        consumer.Received += async (_, ea) =>
        {
            await HandleMessage(ea, cancellationToken);
        };

        _channel!.BasicConsume(
            queue: "driver.inactive",
            autoAck: false,
            consumer: consumer);

        return Task.CompletedTask;
    }

    private async Task HandleMessage(
        BasicDeliverEventArgs eventArgs,
        CancellationToken cancellationToken)
    {
        try
        {
            var msg = JsonSerializer.Deserialize<DriverInactiveMessage>(
                Encoding.UTF8.GetString(eventArgs.Body.ToArray()));

            if (msg == null)
            {
                _channel!.BasicAck(eventArgs.DeliveryTag, false);
                return;
            }

            var chatId = await sessions.GetChatIdByDriverIdAsync(msg.DriverId);

            if (chatId == null)
            {
                _channel!.BasicAck(eventArgs.DeliveryTag, false);
                return;
            }

            var isTracking = await sessions.IsTrackingActiveAsync(chatId.Value);

            if (!isTracking)
            {
                _channel!.BasicAck(eventArgs.DeliveryTag, false);
                return;
            }

            var lastStop = await sessions.GetTrackingStoppedAtAsync(chatId.Value);

            if (lastStop != null &&
                DateTime.UtcNow - lastStop < TimeSpan.FromMinutes(5))
            {
                _channel!.BasicAck(eventArgs.DeliveryTag, false);
                return;
            }

            await tracking.StopTrackingAsync(chatId.Value);

            await bot.SendMessage(
                chatId.Value,
                "⚠️ Схоже, що трекінг зупинився.\n\nБудь ласка, увімкніть геолокацію",
                cancellationToken: cancellationToken);

            await sessions.SaveTrackingStoppedAtAsync(chatId.Value, DateTime.UtcNow);

            _channel!.BasicAck(eventArgs.DeliveryTag, false);
        }
        catch
        {
            _channel!.BasicNack(eventArgs.DeliveryTag, false, true);
        }
    }

    public override void Dispose()
    {
        _channel?.Close();
        _channel?.Dispose();
        base.Dispose();
    }

    private class DriverInactiveMessage
    {
        public Guid DriverId { get; set; }
    }
}
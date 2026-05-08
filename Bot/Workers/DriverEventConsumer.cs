namespace Bot.Workers;

using System.Text;
using System.Text.Json;
using Bot.Interfaces;
using Data.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Telegram.Bot;

/// <summary>
/// Consumes events from RabbitMQ regarding driver inactivity and notifies them via Telegram.
/// </summary>
public class DriverEventConsumer(
    IConnection connection,
    ITrackingService tracking,
    ITelegramBotClient bot,
    IDriverSessionStore sessions,
    ILogger<DriverEventConsumer> logger)
    : BackgroundService
{
    private const string QueueName = "driver.inactive";
    private IModel? _channel;

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            _channel = connection.CreateModel();

            _channel.QueueDeclare(
                queue: QueueName,
                durable: true,
                exclusive: false,
                autoDelete: false);

            _channel.BasicQos(0, 10, false);

            logger.LogInformation("RabbitMQ Consumer started. Listening on queue: {Queue}", QueueName);
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "Failed to initialize RabbitMQ channel");
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
            await HandleMessage(ea, cancellationToken);
        };

        _channel.BasicConsume(
            queue: QueueName,
            autoAck: false,
            consumer: consumer);

        return Task.CompletedTask;
    }

    private async Task HandleMessage(
        BasicDeliverEventArgs eventArgs,
        CancellationToken cancellationToken)
    {
        Guid? currentDriverId = null;
        try
        {
            var body = eventArgs.Body.ToArray();
            var messageJson = Encoding.UTF8.GetString(body);
            var msg = JsonSerializer.Deserialize<DriverInactiveMessage>(messageJson);

            if (msg == null)
            {
                _channel!.BasicAck(eventArgs.DeliveryTag, false);
                return;
            }

            currentDriverId = msg.DriverId;

            var isTracking = await sessions.IsTrackingActiveAsync(msg.DriverId);
            if (!isTracking)
            {
                _channel!.BasicAck(eventArgs.DeliveryTag, false);
                return;
            }

            var lastStop = await sessions.GetTrackingStoppedAtAsync(msg.DriverId);
            if (lastStop != null && DateTime.UtcNow - lastStop < TimeSpan.FromMinutes(5))
            {
                _channel!.BasicAck(eventArgs.DeliveryTag, false);
                return;
            }

            var chatId = await sessions.GetChatIdByDriverIdAsync(msg.DriverId);
            if (chatId != null)
            {
                await tracking.StopTrackingAsync(chatId.Value);

                await bot.SendMessage(
                    chatId.Value,
                    "⚠️ Твою геолокацію втрачено.\n\nБудь ласка, перевір з'єднання та увімкни трансляцію знову 🛰",
                    cancellationToken: cancellationToken);

                logger.LogInformation("Inactivity notification sent to driver {DriverId}", msg.DriverId);
            }

            _channel!.BasicAck(eventArgs.DeliveryTag, false);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing inactivity message for driver {DriverId}", currentDriverId);
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
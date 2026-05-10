namespace Business.Services;

using System.Net;
using System.Text;
using System.Text.Json;
using Business.Interfaces;
using Core.Exceptions;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

/// <summary>
/// Implementation of the notification service using RabbitMQ Topic Exchange.
/// Dispatches events to the 'tms.driver.events' exchange.
/// </summary>
public class NotificationService(IConnection connection, ILogger<NotificationService> logger)
    : INotificationService
{
    private const string ExchangeName = "tms.driver.events";

    /// <inheritdoc />
    public Task PublishDriverInactiveAsync(Guid driverId)
        => SendEventAsync(driverId, null, "driver.inactive");

    /// <inheritdoc />
    public Task PublishLoadAssignedAsync(Guid driverId, Guid loadId)
        => SendEventAsync(driverId, loadId, "load.assigned");

    /// <inheritdoc />
    public Task PublishLoadCanceledAsync(Guid driverId, Guid loadId)
        => SendEventAsync(driverId, loadId, "load.canceled");

    /// <summary>
    /// Internal method to serialize and publish events to RabbitMQ.
    /// </summary>
    /// <param name="driverId">Target driver identifier.</param>
    /// <param name="loadId">Related load identifier (optional).</param>
    /// <param name="routingKey">The RabbitMQ routing key (e.g., load.assigned).</param>
    private async Task SendEventAsync(Guid driverId, Guid? loadId, string routingKey)
    {
        await Task.Run(() =>
        {
            try
            {
                using var channel = connection.CreateModel();

                channel.ExchangeDeclare(exchange: ExchangeName, type: ExchangeType.Topic, durable: true);

                var message = new
                {
                    DriverId = driverId,
                    LoadId = loadId,
                    Timestamp = DateTime.UtcNow,
                    EventType = routingKey,
                };

                var payload = JsonSerializer.Serialize(message);
                var body = Encoding.UTF8.GetBytes(payload);

                var props = channel.CreateBasicProperties();
                props.Persistent = true;

                channel.BasicPublish(
                    exchange: ExchangeName,
                    routingKey: routingKey,
                    basicProperties: props,
                    body: body);

                logger.LogInformation("Successfully published {Key} for Driver {DId}", routingKey, driverId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to publish message to RabbitMQ: {Key}", routingKey);
                throw new TmsException("External notification system is unavailable", ex, HttpStatusCode.InternalServerError);
            }
        });
    }
}
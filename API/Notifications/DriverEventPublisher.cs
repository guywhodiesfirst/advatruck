namespace API.Notifications;

using System.Net;
using System.Text;
using System.Text.Json;
using Core.Exceptions;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

public class DriverEventPublisher(IConnection connection, ILogger<DriverEventPublisher> logger)
{
    public void PublishDriverInactive(Guid driverId)
    {
        try
        {
            using var channel = connection.CreateModel();

            channel.QueueDeclare(
                queue: "driver.inactive",
                durable: true,
                exclusive: false,
                autoDelete: false);

            var payload = JsonSerializer.Serialize(new DriverInactiveMessage
            {
                DriverId = driverId,
            });

            var body = Encoding.UTF8.GetBytes(payload);

            var props = channel.CreateBasicProperties();
            props.Persistent = true;

            channel.BasicPublish(
                exchange: string.Empty,
                routingKey: "driver.inactive",
                basicProperties: props,
                body: body);

            logger.LogInformation("Successfully published inactivity event for driver {DriverId}", driverId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to publish driver inactivity event to RabbitMQ for driver {DriverId}", driverId);
            throw new TmsException("Failed to notify internal systems about driver inactivity", ex, HttpStatusCode.InternalServerError);
        }
    }

    private class DriverInactiveMessage
    {
        public Guid DriverId { get; set; }
    }
}
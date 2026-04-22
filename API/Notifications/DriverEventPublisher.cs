namespace API.Notifications;

using System.Text;
using System.Text.Json;
using RabbitMQ.Client;

public class DriverEventPublisher(IConnection connection)
{
    public void PublishDriverInactive(Guid driverId)
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
    }

    private class DriverInactiveMessage
    {
        public Guid DriverId { get; set; }
    }
}
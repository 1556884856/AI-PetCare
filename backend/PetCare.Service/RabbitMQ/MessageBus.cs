using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using PetCare.Core.Interfaces;
using RabbitMQ.Client;

namespace PetCare.Service.RabbitMQ;

public class MessageBus : IMessageBus
{
    private readonly RabbitMQConnection _connection;
    private readonly string _exchangeName;

    public MessageBus(RabbitMQConnection connection, IConfiguration configuration)
    {
        _connection = connection;
        _exchangeName = configuration["RabbitMQ:ExchangeName"] ?? "petcare.events";
    }

    public void Publish<T>(string routingKey, T message)
    {
        var json = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(json);

        var props = _connection.Channel.CreateBasicProperties();
        props.ContentType = "application/json";
        props.DeliveryMode = 2;
        props.MessageId = Guid.NewGuid().ToString();
        props.Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds());

        _connection.Channel.BasicPublish(
            exchange: _exchangeName,
            routingKey: routingKey,
            basicProperties: props,
            body: body
        );
    }
}

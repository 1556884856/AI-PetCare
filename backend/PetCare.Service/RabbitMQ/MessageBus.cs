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

    public async Task PublishAsync<T>(string routingKey, T message)
    {
        var json = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(json);

        var channel = await _connection.GetChannelAsync();
        var props = new BasicProperties
        {
            ContentType = "application/json",
            DeliveryMode = DeliveryModes.Persistent,
            MessageId = Guid.NewGuid().ToString(),
        };

        await channel.BasicPublishAsync(
            exchange: _exchangeName,
            routingKey: routingKey,
            mandatory: false,
            basicProperties: props,
            body: body
        );
    }
}

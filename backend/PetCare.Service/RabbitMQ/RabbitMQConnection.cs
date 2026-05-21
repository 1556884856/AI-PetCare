using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

namespace PetCare.Service.RabbitMQ;

public class RabbitMQConnection : IDisposable
{
    private readonly ConnectionFactory _factory;
    private readonly string _exchangeName;
    private IConnection? _connection;
    private IModel? _channel;
    private readonly object _lock = new();

    public IModel Channel
    {
        get
        {
            EnsureConnected();
            return _channel!;
        }
    }

    public RabbitMQConnection(IConfiguration configuration)
    {
        _factory = new ConnectionFactory
        {
            HostName = configuration["RabbitMQ:HostName"] ?? "localhost",
            Port = int.Parse(configuration["RabbitMQ:Port"] ?? "5672"),
            UserName = configuration["RabbitMQ:UserName"] ?? "guest",
            Password = configuration["RabbitMQ:Password"] ?? "guest",
            DispatchConsumersAsync = true
        };
        _exchangeName = configuration["RabbitMQ:ExchangeName"] ?? "petcare.events";
    }

    private void EnsureConnected()
    {
        if (_channel != null && _channel.IsOpen) return;

        lock (_lock)
        {
            if (_channel != null && _channel.IsOpen) return;

            _channel?.Dispose();
            _connection?.Dispose();

            _connection = _factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.ExchangeDeclare(_exchangeName, ExchangeType.Topic, durable: true, autoDelete: false);
        }
    }

    public bool TryConnect()
    {
        try
        {
            EnsureConnected();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public void Dispose()
    {
        _channel?.Close();
        _channel?.Dispose();
        _connection?.Close();
        _connection?.Dispose();
    }
}

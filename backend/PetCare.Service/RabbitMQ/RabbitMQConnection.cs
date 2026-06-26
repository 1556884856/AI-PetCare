using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

namespace PetCare.Service.RabbitMQ;

public class RabbitMQConnection : IAsyncDisposable
{
    private readonly ConnectionFactory _factory;
    private readonly string _exchangeName;
    private IConnection? _connection;
    private IChannel? _channel;
    private readonly SemaphoreSlim _lock = new(1, 1);

    public RabbitMQConnection(IConfiguration configuration)
    {
        _factory = new ConnectionFactory
        {
            HostName = configuration["RabbitMQ:HostName"] ?? "localhost",
            Port = int.Parse(configuration["RabbitMQ:Port"] ?? "5672"),
            UserName = configuration["RabbitMQ:UserName"] ?? "guest",
            Password = configuration["RabbitMQ:Password"] ?? "guest",
            
        };
        _exchangeName = configuration["RabbitMQ:ExchangeName"] ?? "petcare.events";
    }

    public async Task<IChannel> GetChannelAsync(CancellationToken ct = default)
    {
        if (_channel != null) return _channel;

        await _lock.WaitAsync(ct);
        try
        {
            if (_channel != null) return _channel;

            if (_channel != null) { await _channel.CloseAsync(ct); await _channel.DisposeAsync(); }
            if (_connection != null) { await _connection.CloseAsync(ct); await _connection.DisposeAsync(); }

            _connection = await _factory.CreateConnectionAsync(ct);
            _channel = await _connection.CreateChannelAsync(cancellationToken: ct);
            await _channel.ExchangeDeclareAsync(_exchangeName, ExchangeType.Topic, durable: true, autoDelete: false, cancellationToken: ct);
        }
        finally { _lock.Release(); }
        return _channel;
    }

    public async Task<bool> TryConnectAsync()
    {
        try
        {
            await GetChannelAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_channel != null) { await _channel.CloseAsync(); await _channel.DisposeAsync(); }
        if (_connection != null) { await _connection.CloseAsync(); await _connection.DisposeAsync(); }
        _lock.Dispose();
    }
}

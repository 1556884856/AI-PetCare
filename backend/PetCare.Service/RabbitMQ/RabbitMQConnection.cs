using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
namespace PetCare.Service.RabbitMQ;
/// <summary>
/// RabbitMQ 连接管理，使用懒加载的双重检查锁模式创建连接和信道。
/// 实现 IAsyncDisposable 以在应用关闭时正确释放资源。
/// </summary>
public class RabbitMQConnection : IAsyncDisposable
{
    private readonly ConnectionFactory _factory;
    private readonly string _exchangeName;
    private IConnection? _connection;
    private IChannel? _channel;
    private readonly SemaphoreSlim _lock = new(1, 1);
    public RabbitMQConnection(IConfiguration configuration)
    {
        _factory = new ConnectionFactory { HostName = configuration["RabbitMQ:HostName"] ?? "localhost", Port = int.Parse(configuration["RabbitMQ:Port"] ?? "5672"), UserName = configuration["RabbitMQ:UserName"] ?? "guest", Password = configuration["RabbitMQ:Password"] ?? "guest" };
        _exchangeName = configuration["RabbitMQ:ExchangeName"] ?? "petcare.events";
    }
    /// <summary>获取或创建 RabbitMQ 信道（双重检查锁 + 信号量）</summary>
    public async Task<IChannel> GetChannelAsync(CancellationToken ct = default)
    {
        if (_channel != null) return _channel;
        await _lock.WaitAsync(ct);
        try
        {
            if (_channel != null) return _channel;
            _connection = await _factory.CreateConnectionAsync(ct);
            _channel = await _connection.CreateChannelAsync(cancellationToken: ct);
            await _channel.ExchangeDeclareAsync(_exchangeName, ExchangeType.Topic, durable: true, autoDelete: false, cancellationToken: ct);
        }
        finally { _lock.Release(); }
        return _channel;
    }
    /// <summary>尝试连接 RabbitMQ，用于消费者重试</summary>
    public async Task<bool> TryConnectAsync()
    {
        try { await GetChannelAsync(); return true; }
        catch { return false; }
    }
    public async ValueTask DisposeAsync()
    {
        if (_channel != null) { await _channel.CloseAsync(); await _channel.DisposeAsync(); }
        if (_connection != null) { await _connection.CloseAsync(); await _connection.DisposeAsync(); }
        _lock.Dispose();
    }
}

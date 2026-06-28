using StackExchange.Redis;
using Microsoft.Extensions.Configuration;
namespace PetCare.Service.Redis;
/// <summary>
/// Redis 连接管理，封装 ConnectionMultiplexer 的懒加载和自动重连。
/// 用于优惠券库存控制、通知未读数缓存、分布式锁等场景。
/// </summary>
public class RedisConnection : IDisposable
{
    private readonly string _connectionString;
    private readonly int _database;
    private ConnectionMultiplexer? _redis;
    private IDatabase? _db;
    private readonly object _lock = new();
    public IDatabase Database
    {
        get { EnsureConnected(); return _db!; }
    }
    public RedisConnection(IConfiguration configuration)
    {
        _connectionString = configuration["Redis:ConnectionString"] ?? "localhost:6379";
        _database = int.Parse(configuration["Redis:DefaultDatabase"] ?? "0");
    }
    /// <summary>确保连接已建立（双重检查锁 + 自动重连）</summary>
    private void EnsureConnected()
    {
        if (_redis != null && _redis.IsConnected) return;
        lock (_lock)
        {
            if (_redis != null && _redis.IsConnected) return;
            _redis?.Dispose();
            _redis = ConnectionMultiplexer.Connect(_connectionString);
            _db = _redis.GetDatabase(_database);
        }
    }
    public bool TryConnect()
    {
        try { EnsureConnected(); return true; }
        catch { return false; }
    }
    public ISubscriber GetSubscriber()
    {
        EnsureConnected();
        return _redis!.GetSubscriber();
    }
    public void Dispose()
    {
        _redis?.Close();
        _redis?.Dispose();
    }
}

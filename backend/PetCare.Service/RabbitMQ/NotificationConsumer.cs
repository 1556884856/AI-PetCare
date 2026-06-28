using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PetCare.Core.Events;
using PetCare.Core.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using PetCare.Service.Redis;
using StackExchange.Redis;
namespace PetCare.Service.RabbitMQ;
/// <summary>
/// 通知消息消费者（BackgroundService），持续监听 RabbitMQ 并处理消息。
/// 处理预约创建/状态变更、支付完成、优惠券领取等事件，创建对应的用户通知。
/// 通过 Redis 存储已处理消息的 MessageId，实现消息去重（7天过期）。
/// </summary>
public class NotificationConsumer : BackgroundService
{
    private readonly RabbitMQConnection _connection;
    private readonly RedisConnection _redis;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<NotificationConsumer> _logger;
    private readonly string _exchangeName;
    private readonly string _queueName;
    public NotificationConsumer(RabbitMQConnection connection, RedisConnection redis, IServiceScopeFactory scopeFactory, IConfiguration configuration, ILogger<NotificationConsumer> logger)
    {
        _connection = connection; _redis = redis; _scopeFactory = scopeFactory; _logger = logger;
        _exchangeName = configuration["RabbitMQ:ExchangeName"] ?? "petcare.events";
        _queueName = (configuration["RabbitMQ:QueuePrefix"] ?? "petcare") + ".notification";
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                if (!await _connection.TryConnectAsync()) { _logger.LogWarning("NotificationConsumer: RabbitMQ not available, retrying in 5s..."); await Task.Delay(5000, stoppingToken); continue; }
                var channel = await _connection.GetChannelAsync();
                await channel.QueueDeclareAsync(_queueName, durable: true, exclusive: false, autoDelete: false);
                await channel.QueueBindAsync(_queueName, _exchangeName, "appointment.*");
                await channel.QueueBindAsync(_queueName, _exchangeName, "payment.completed");
                await channel.QueueBindAsync(_queueName, _exchangeName, "notification.bulk");
                await channel.QueueBindAsync(_queueName, _exchangeName, "coupon.received");
                var consumer = new AsyncEventingBasicConsumer(channel);
                consumer.ReceivedAsync += async (sender, ea) =>
                {
                    try
                    {
                        var body = Encoding.UTF8.GetString(ea.Body.ToArray());
                        _logger.LogInformation("NotificationConsumer received: {RoutingKey}", ea.RoutingKey);
                        // 消息去重：通过 MessageId + Redis SETNX 防止重复消费
                        var msgId = ea.BasicProperties?.MessageId;
                        if (!string.IsNullOrEmpty(msgId))
                        {
                            var dedupKey = $"msg:dedup:{msgId}";
                            var isNew = await _redis.Database.StringSetAsync(dedupKey, "1", TimeSpan.FromDays(7), When.NotExists);
                            if (!isNew) { _logger.LogWarning("Duplicate message {MsgId}, skipping", msgId); await channel.BasicAckAsync(ea.DeliveryTag, false); return; }
                        }
                        using var scope = _scopeFactory.CreateScope();
                        var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();
                        switch (ea.RoutingKey)
                        {
                            case "appointment.created":
                                var created = JsonSerializer.Deserialize<AppointmentCreatedEvent>(body);
                                if (created != null)
                                    await notificationService.CreateNotificationAsync(created.UserId, "预约成功", $"您的宠物 {created.PetName} 的{created.ServiceName} 预约已提交，时间为 {created.AppointmentDate:yyyy-MM-dd} {created.TimeSlot}，请及时支付。", 0, created.AppointmentId);
                                break;
                            case "appointment.status":
                                var statusEvent = JsonSerializer.Deserialize<AppointmentStatusEvent>(body);
                                if (statusEvent != null)
                                    await notificationService.CreateNotificationAsync(statusEvent.UserId, "预约状态更新", statusEvent.Message, 1, statusEvent.AppointmentId);
                                break;
                            case "payment.completed":
                                var payEvent = JsonSerializer.Deserialize<PaymentCompletedEvent>(body);
                                if (payEvent != null)
                                    await notificationService.CreateNotificationAsync(payEvent.UserId, "支付成功", $"您的预约已支付 ¥{payEvent.Amount:F2}，我们将按时为您服务！", 0, payEvent.AppointmentId);
                                break;
                            case "coupon.received":
                                var couponEvent = JsonSerializer.Deserialize<CouponReceivedEvent>(body);
                                if (couponEvent != null && couponEvent.UserId > 0)
                                    await notificationService.CreateNotificationAsync(couponEvent.UserId, "优惠券到账", $"您获得了一张{couponEvent.CouponName} (¥{couponEvent.Value})，快去使用吧！", 2, null);
                                break;
                        }
                        await channel.BasicAckAsync(ea.DeliveryTag, false);
                    }
                    catch (Exception ex) { _logger.LogError(ex, "NotificationConsumer message error"); try { await channel.BasicNackAsync(ea.DeliveryTag, false, true); } catch { } }
                };
                await channel.BasicConsumeAsync(_queueName, false, consumer);
                try { await Task.Delay(Timeout.Infinite, stoppingToken); } catch (OperationCanceledException) { }
            }
            catch (Exception ex) { _logger.LogError(ex, "NotificationConsumer connection error, retrying..."); await Task.Delay(5000, stoppingToken); }
        }
    }
}

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

namespace PetCare.Service.RabbitMQ;

public class NotificationConsumer : BackgroundService
{
    private readonly RabbitMQConnection _connection;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<NotificationConsumer> _logger;
    private readonly string _exchangeName;
    private readonly string _queueName;

    public NotificationConsumer(RabbitMQConnection connection, IServiceScopeFactory scopeFactory, IConfiguration configuration, ILogger<NotificationConsumer> logger)
    {
        _connection = connection;
        _scopeFactory = scopeFactory;
        _logger = logger;
        _exchangeName = configuration["RabbitMQ:ExchangeName"] ?? "petcare.events";
        _queueName = (configuration["RabbitMQ:QueuePrefix"] ?? "petcare") + ".notification";
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                if (!_connection.TryConnect())
                {
                    _logger.LogWarning("NotificationConsumer: RabbitMQ not available, retrying in 5s...");
                    await Task.Delay(5000, stoppingToken);
                    continue;
                }

                var channel = _connection.Channel;

                channel.QueueDeclare(_queueName, durable: true, exclusive: false, autoDelete: false);
                channel.QueueBind(_queueName, _exchangeName, "appointment.*");
                channel.QueueBind(_queueName, _exchangeName, "payment.completed");
                channel.QueueBind(_queueName, _exchangeName, "notification.bulk");
                channel.QueueBind(_queueName, _exchangeName, "coupon.received");

                var consumer = new AsyncEventingBasicConsumer(channel);
                consumer.Received += async (_, ea) =>
                {
                    try
                    {
                        var body = Encoding.UTF8.GetString(ea.Body.ToArray());
                        _logger.LogInformation("NotificationConsumer received: {RoutingKey}", ea.RoutingKey);

                        using var scope = _scopeFactory.CreateScope();
                        var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

                        switch (ea.RoutingKey)
                        {
                            case "appointment.created":
                                var created = JsonSerializer.Deserialize<AppointmentCreatedEvent>(body);
                                if (created != null)
                                {
                                    await notificationService.CreateNotificationAsync(
                                        created.UserId, "预约成功",
                                        $"您的宠物 {created.PetName} 的 {created.ServiceName} 预约已提交，时间为 {created.AppointmentDate:yyyy-MM-dd} {created.TimeSlot}，请及时支付。",
                                        0, created.AppointmentId);
                                }
                                break;

                            case "appointment.status":
                                var statusEvent = JsonSerializer.Deserialize<AppointmentStatusEvent>(body);
                                if (statusEvent != null)
                                {
                                    await notificationService.CreateNotificationAsync(
                                        statusEvent.UserId, "预约状态更新", statusEvent.Message,
                                        1, statusEvent.AppointmentId);
                                }
                                break;

                            case "payment.completed":
                                var payEvent = JsonSerializer.Deserialize<PaymentCompletedEvent>(body);
                                if (payEvent != null)
                                {
                                    await notificationService.CreateNotificationAsync(
                                        payEvent.UserId, "支付成功",
                                        $"您的预约已支付 ¥{payEvent.Amount:F2}，我们将按时为您服务！",
                                        0, payEvent.AppointmentId);
                                }
                                break;

                            case "coupon.received":
                                var couponEvent = JsonSerializer.Deserialize<CouponReceivedEvent>(body);
                                if (couponEvent != null && couponEvent.UserId > 0)
                                {
                                    await notificationService.CreateNotificationAsync(
                                        couponEvent.UserId, "优惠券到账",
                                        $"您获得了一张 {couponEvent.CouponName} (¥{couponEvent.Value})，快去使用吧！",
                                        2, null);
                                }
                                break;
                        }

                        channel.BasicAck(ea.DeliveryTag, false);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "NotificationConsumer message error");
                        try { channel.BasicNack(ea.DeliveryTag, false, true); } catch { }
                    }
                };

                channel.BasicConsume(_queueName, false, consumer);

                // 保持连接直到取消
                try { await Task.Delay(Timeout.Infinite, stoppingToken); }
                catch (OperationCanceledException) { }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "NotificationConsumer connection error, retrying...");
                await Task.Delay(5000, stoppingToken);
            }
        }
    }
}

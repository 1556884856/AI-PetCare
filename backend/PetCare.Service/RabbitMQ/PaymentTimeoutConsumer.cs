using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PetCare.Core.Events;
using PetCare.Data;
using PetCare.Core.Enums;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
namespace PetCare.Service.RabbitMQ;
/// <summary>
/// 支付超时消费者（BackgroundService）。
/// 支付单创建时发布 30 分钟延迟消息（通过死信队列实现），
/// 超时后自动将未支付订单状态设为已退款，预约设为已取消。
/// </summary>
public class PaymentTimeoutConsumer : BackgroundService
{
    private readonly RabbitMQConnection _connection;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<PaymentTimeoutConsumer> _logger;
    private readonly string _exchangeName;
    private readonly string _queueName;
    private readonly string _dlxName;
    private readonly string _dlxQueueName;
    public PaymentTimeoutConsumer(RabbitMQConnection connection, IServiceScopeFactory scopeFactory, IConfiguration configuration, ILogger<PaymentTimeoutConsumer> logger)
    {
        _connection = connection; _scopeFactory = scopeFactory; _logger = logger;
        var prefix = configuration["RabbitMQ:QueuePrefix"] ?? "petcare";
        _exchangeName = configuration["RabbitMQ:ExchangeName"] ?? "petcare.events";
        _queueName = prefix + ".payment.timeout";
        _dlxName = prefix + ".payment.dlx";
        _dlxQueueName = prefix + ".payment.timeout.dlx";
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                if (!await _connection.TryConnectAsync()) { _logger.LogWarning("PaymentTimeoutConsumer: RabbitMQ not available, retrying in 5s..."); await Task.Delay(5000, stoppingToken); continue; }
                var channel = await _connection.GetChannelAsync();
                // 死信交换机声明
                await channel.ExchangeDeclareAsync(_dlxName, ExchangeType.Topic, durable: true);
                // 主队列：设置 30 分钟 TTL，超时消息路由到死信队列
                var args = new Dictionary<string, object?>() { { "x-dead-letter-exchange", _dlxName }, { "x-dead-letter-routing-key", "payment.timeout" }, { "x-message-ttl", 1800000 } };
                await channel.QueueDeclareAsync(_queueName, durable: true, exclusive: false, autoDelete: false, args);
                await channel.QueueBindAsync(_queueName, _exchangeName, "payment.timeout");
                // 死信队列
                await channel.QueueDeclareAsync(_dlxQueueName, durable: true, exclusive: false, autoDelete: false);
                await channel.QueueBindAsync(_dlxQueueName, _dlxName, "payment.timeout");
                var consumer = new AsyncEventingBasicConsumer(channel);
                consumer.ReceivedAsync += async (sender, ea) =>
                {
                    try
                    {
                        var body = Encoding.UTF8.GetString(ea.Body.ToArray());
                        var timeoutEvent = JsonSerializer.Deserialize<PaymentTimeoutEvent>(body);
                        if (timeoutEvent != null)
                        {
                            using var scope = _scopeFactory.CreateScope();
                            var db = scope.ServiceProvider.GetRequiredService<PetCareDbContext>();
                            var payment = await db.Payments.FindAsync(timeoutEvent.PaymentId);
                            // 仅取消仍处于待支付状态的支付单
                            if (payment != null && payment.Status == PaymentStatus.Pending)
                            {
                                payment.Status = PaymentStatus.Refunded;
                                var appointment = await db.Appointments.FindAsync(timeoutEvent.AppointmentId);
                                if (appointment != null) appointment.Status = AppointmentStatus.Cancelled;
                                await db.SaveChangesAsync();
                                _logger.LogInformation("Payment {PaymentId} timeout, auto-cancelled", timeoutEvent.PaymentId);
                            }
                        }
                        await channel.BasicAckAsync(ea.DeliveryTag, false);
                    }
                    catch (Exception ex) { _logger.LogError(ex, "PaymentTimeoutConsumer message error"); try { await channel.BasicNackAsync(ea.DeliveryTag, false, true); } catch { } }
                };
                await channel.BasicConsumeAsync(_dlxQueueName, false, consumer);
                try { await Task.Delay(Timeout.Infinite, stoppingToken); } catch (OperationCanceledException) { }
            }
            catch (Exception ex) { _logger.LogError(ex, "PaymentTimeoutConsumer connection error, retrying..."); await Task.Delay(5000, stoppingToken); }
        }
    }
}

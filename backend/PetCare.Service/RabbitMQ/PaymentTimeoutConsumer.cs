using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PetCare.Core.Events;
using PetCare.Data;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace PetCare.Service.RabbitMQ;

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
        _connection = connection;
        _scopeFactory = scopeFactory;
        _logger = logger;
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
                if (!_connection.TryConnect())
                {
                    _logger.LogWarning("PaymentTimeoutConsumer: RabbitMQ not available, retrying in 5s...");
                    await Task.Delay(5000, stoppingToken);
                    continue;
                }

                var channel = _connection.Channel;

                channel.ExchangeDeclare(_dlxName, ExchangeType.Topic, durable: true);

                var args = new Dictionary<string, object>
                {
                    { "x-dead-letter-exchange", _dlxName },
                    { "x-dead-letter-routing-key", "payment.timeout" },
                    { "x-message-ttl", 1800000 }
                };
                channel.QueueDeclare(_queueName, durable: true, exclusive: false, autoDelete: false, args);
                channel.QueueBind(_queueName, _exchangeName, "payment.timeout");

                channel.QueueDeclare(_dlxQueueName, durable: true, exclusive: false, autoDelete: false);
                channel.QueueBind(_dlxQueueName, _dlxName, "payment.timeout");

                var consumer = new AsyncEventingBasicConsumer(channel);
                consumer.Received += async (_, ea) =>
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
                            if (payment != null && payment.Status == 0)
                            {
                                payment.Status = 2;
                                var appointment = await db.Appointments.FindAsync(timeoutEvent.AppointmentId);
                                if (appointment != null) appointment.Status = 3;
                                await db.SaveChangesAsync();
                                _logger.LogInformation("Payment {PaymentId} timeout, auto-cancelled", timeoutEvent.PaymentId);
                            }
                        }

                        channel.BasicAck(ea.DeliveryTag, false);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "PaymentTimeoutConsumer message error");
                        try { channel.BasicNack(ea.DeliveryTag, false, true); } catch { }
                    }
                };

                channel.BasicConsume(_dlxQueueName, false, consumer);

                try { await Task.Delay(Timeout.Infinite, stoppingToken); }
                catch (OperationCanceledException) { }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PaymentTimeoutConsumer connection error, retrying...");
                await Task.Delay(5000, stoppingToken);
            }
        }
    }
}

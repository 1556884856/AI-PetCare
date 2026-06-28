namespace PetCare.Core.Interfaces;

/// <summary>
/// 消息总线接口，用于发布领域事件到 RabbitMQ。
/// 支持按 RoutingKey 将消息路由到不同的消费者队列。
/// </summary>
public interface IMessageBus
{
    /// <summary>发布一条消息到指定路由键对应的队列</summary>
    /// <typeparam name="T">消息类型</typeparam>
    /// <param name="routingKey">路由键，用于 RabbitMQ Topic Exchange 路由</param>
    /// <param name="message">消息体（将序列化为 JSON）</param>
    Task PublishAsync<T>(string routingKey, T message);
}

namespace PetCare.Core.Interfaces;

public interface IMessageBus
{
    void Publish<T>(string routingKey, T message);
}

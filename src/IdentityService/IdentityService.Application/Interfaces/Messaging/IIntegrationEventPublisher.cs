namespace IdentityService.Application.Interfaces.Messaging;

public interface IIntegrationEventPublisher
{
    Task PublishAsync<TEvent>(TEvent @event, string eventName)
        where TEvent : class;
}
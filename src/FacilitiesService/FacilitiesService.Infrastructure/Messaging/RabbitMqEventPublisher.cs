using System.Text.Json;
using FacilitiesService.Application.Interfaces.Messaging;
using RabbitMQ.Client;

namespace FacilitiesService.Infrastructure.Messaging;

public class RabbitMqEventPublisher : IIntegrationEventPublisher, IAsyncDisposable
{
    private IConnection? _connection;
    private IChannel? _channel;

    private const string ExchangeName = "employee.onboarding";
    private readonly SemaphoreSlim _connectionLock = new(1, 1);

    private async Task EnsureConnectionAsync()
    {
        if (_channel is not null) return;

        await _connectionLock.WaitAsync();
        try
        {
            if (_channel is not null) return;

            var factory = new ConnectionFactory
            {
                HostName = "localhost",
                AutomaticRecoveryEnabled = true
            };

            _connection = await factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();

            await _channel.ExchangeDeclareAsync(
                exchange: ExchangeName,
                type: ExchangeType.Topic,
                durable: true);
        }
        finally
        {
            _connectionLock.Release();
        }
    }

    public async Task PublishAsync<TEvent>(TEvent @event, string eventName)
        where TEvent : class
    {
        await EnsureConnectionAsync();

        var body = JsonSerializer.SerializeToUtf8Bytes(@event);

        var properties = new BasicProperties
        {
            ContentType = "application/json",
            DeliveryMode = DeliveryModes.Persistent
        };

        await _channel!.BasicPublishAsync(
            exchange: ExchangeName,
            routingKey: eventName,
            mandatory: false,
            basicProperties: properties,
            body: body);
    }

    public async ValueTask DisposeAsync()
    {
        if (_channel is not null)
        {
            await _channel.CloseAsync();
            await _channel.DisposeAsync();
        }

        if (_connection is not null)
        {
            await _connection.CloseAsync();
            _connection.Dispose();
        }
    }
}
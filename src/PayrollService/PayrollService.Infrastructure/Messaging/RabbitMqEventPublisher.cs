using System.Text.Json;
using PayrollService.Application.Interfaces.Messaging;
using RabbitMQ.Client;

namespace PayrollService.Infrastructure.Messaging;

public class RabbitMqEventPublisher : IIntegrationEventPublisher, IAsyncDisposable
{
    private IConnection? _connection;
    private IChannel? _channel;
    private const string ExchangeName = "employee.onboarding";
    private readonly SemaphoreSlim _lock = new(1, 1);

    private async Task EnsureAsync()
    {
        if (_channel != null) return;

        await _lock.WaitAsync();
        try
        {
            if (_channel != null) return;

            var factory = new ConnectionFactory
            {
                HostName = "localhost",
                AutomaticRecoveryEnabled = true
            };

            _connection = await factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();

            await _channel.ExchangeDeclareAsync(
                ExchangeName,
                ExchangeType.Topic,
                durable: true);
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task PublishAsync<TEvent>(TEvent @event, string eventName)
        where TEvent : class
    {
        await EnsureAsync();

        var body = JsonSerializer.SerializeToUtf8Bytes(@event);

        var props = new BasicProperties
        {
            ContentType = "application/json",
            DeliveryMode = DeliveryModes.Persistent
        };

        await _channel!.BasicPublishAsync(
            ExchangeName,
            eventName,
            false,
            props,
            body);
    }

    public async ValueTask DisposeAsync()
    {
        if (_channel != null)
        {
            await _channel.CloseAsync();
            await _channel.DisposeAsync();
        }

        if (_connection != null)
        {
            await _connection.CloseAsync();
            _connection.Dispose();
        }
    }
}
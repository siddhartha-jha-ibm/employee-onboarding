using System.Text.Json;
using IdentityService.Application.Events;
using IdentityService.Application.Interfaces.Services;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace IdentityService.Api.BackgroundServices;

public class AccessRevokeConsumer : BackgroundService
{
    private readonly IAccessRevokeService _service;
    private IConnection? _connection;
    private IChannel? _channel;

    public AccessRevokeConsumer(IAccessRevokeService service)
    {
        _service = service;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = "localhost",
            AutomaticRecoveryEnabled = true
        };

        _connection = await factory.CreateConnectionAsync(stoppingToken);
        _channel = await _connection.CreateChannelAsync();

        await _channel.ExchangeDeclareAsync(
            "employee.onboarding",
            ExchangeType.Topic,
            true,
            cancellationToken: stoppingToken);

        var queue = await _channel.QueueDeclareAsync(
            cancellationToken: stoppingToken);

        await _channel.QueueBindAsync(
            queue.QueueName,
            "employee.onboarding",
            "access.revoke",
            cancellationToken: stoppingToken);

        var consumer = new AsyncEventingBasicConsumer(_channel);

        consumer.ReceivedAsync += async (_, ea) =>
        {
            var @event =
                JsonSerializer.Deserialize<AccessRevokeIntegrationEvent>(
                    ea.Body.Span);

            if (@event != null)
            {
                await _service.RevokeAsync(@event);
                await _channel.BasicAckAsync(ea.DeliveryTag, false);
            }
        };

        await _channel.BasicConsumeAsync(
            queue.QueueName,
            false,
            consumer,
            cancellationToken: stoppingToken);
    }
}
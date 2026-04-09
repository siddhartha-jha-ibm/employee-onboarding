using System.Text.Json;
using IdentityService.Application.Events;
using IdentityService.Application.Interfaces.Services;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace IdentityService.Api.BackgroundServices;

public class EmployeeCreatedConsumer : BackgroundService
{
    private readonly IIdentityProvisioningService _service;
    private IConnection? _connection;
    private IChannel? _channel;

    public EmployeeCreatedConsumer(IIdentityProvisioningService service)
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
            exchange: "employee.onboarding",
            type: ExchangeType.Topic,
            durable: true,
            cancellationToken: stoppingToken);

        var queue = await _channel.QueueDeclareAsync(cancellationToken: stoppingToken);

        await _channel.QueueBindAsync(
            queue.QueueName,
            "employee.onboarding",
            "employee.created",
            cancellationToken: stoppingToken);

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += async (_, ea) =>
        {
            var @event = JsonSerializer.Deserialize<EmployeeCreatedIntegrationEvent>(
                ea.Body.Span);

            if (@event is not null)
            {
                await _service.ProvisionAccessAsync(@event);
                await _channel.BasicAckAsync(ea.DeliveryTag, false);
            }
        };

        await _channel.BasicConsumeAsync(
            queue: queue.QueueName,
            autoAck: false,
            consumer: consumer,
            cancellationToken: stoppingToken);
    }
}
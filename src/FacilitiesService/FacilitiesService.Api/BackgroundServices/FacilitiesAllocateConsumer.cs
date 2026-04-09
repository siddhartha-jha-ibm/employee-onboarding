using System.Text.Json;
using FacilitiesService.Application.Events;
using FacilitiesService.Application.Interfaces.Services;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace FacilitiesService.Api.BackgroundServices;

public class FacilitiesAllocateConsumer : BackgroundService
{
    private readonly IFacilitiesAllocationService _service;
    private IConnection? _connection;
    private IChannel? _channel;

    public FacilitiesAllocateConsumer(IFacilitiesAllocationService service)
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

        var queue = await _channel.QueueDeclareAsync(
            cancellationToken: stoppingToken);

        await _channel.QueueBindAsync(
            queue: queue.QueueName,
            exchange: "employee.onboarding",
            routingKey: "facilities.allocate",
            cancellationToken: stoppingToken);

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += async (_, ea) =>
        {
            var @event = JsonSerializer
                .Deserialize<FacilitiesAllocateIntegrationEvent>(
                    ea.Body.Span);

            if (@event != null)
            {
                await _service.AllocateAsync(@event);

                await _channel.BasicAckAsync(
                    ea.DeliveryTag,
                    multiple: false);
            }
        };

        await _channel.BasicConsumeAsync(
            queue: queue.QueueName,
            autoAck: false,
            consumer: consumer,
            cancellationToken: stoppingToken);
    }
}
using System.Text.Json;
using FacilitiesService.Application.Events;
using FacilitiesService.Application.Interfaces.Services;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace FacilitiesService.Api.BackgroundServices;

public class FacilitiesReleaseConsumer : BackgroundService
{
    private readonly IFacilitiesReleaseService _service;
    private IConnection? _connection;
    private IChannel? _channel;

    public FacilitiesReleaseConsumer(IFacilitiesReleaseService service)
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
            "facilities.release",
            cancellationToken: stoppingToken);

        var consumer = new AsyncEventingBasicConsumer(_channel);

        consumer.ReceivedAsync += async (_, ea) =>
        {
            var @event =
                JsonSerializer.Deserialize<FacilitiesReleaseIntegrationEvent>(
                    ea.Body.Span);

            if (@event != null)
            {
                await _service.ReleaseAsync(@event);
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

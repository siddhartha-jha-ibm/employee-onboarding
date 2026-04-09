using System.Text.Json;
using EmployeeService.Application.Events;
using EmployeeService.Application.Interfaces.Services;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace EmployeeService.Api.BackgroundServices;

public class FacilitiesReleasedConsumer : BackgroundService
{
    private readonly IOnboardingSagaService _sagaService;
    private IConnection? _connection;
    private IChannel? _channel;

    public FacilitiesReleasedConsumer(IOnboardingSagaService sagaService)
    {
        _sagaService = sagaService;
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
            "facilities.released",
            cancellationToken: stoppingToken);

        var consumer = new AsyncEventingBasicConsumer(_channel);

        consumer.ReceivedAsync += async (_, ea) =>
        {
            var @event =
                JsonSerializer.Deserialize<FacilitiesReleasedIntegrationEvent>(
                    ea.Body.Span);

            if (@event != null)
            {
                await _sagaService.HandleFacilitiesReleasedAsync(@event);
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
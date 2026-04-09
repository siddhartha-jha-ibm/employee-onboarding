using System.Text.Json;
using EmployeeService.Application.Events;
using EmployeeService.Application.Interfaces.Services;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace EmployeeService.Api.BackgroundServices;

public class PayrollActivatedConsumer : BackgroundService
{
    private readonly IOnboardingSagaService _sagaService;
    private IConnection? _connection;
    private IChannel? _channel;

    public PayrollActivatedConsumer(IOnboardingSagaService sagaService)
    {
        _sagaService = sagaService;
    }

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = "localhost",
            AutomaticRecoveryEnabled = true
        };

        _connection =
            await factory.CreateConnectionAsync(stoppingToken);
        _channel =
            await _connection.CreateChannelAsync();

        await _channel.ExchangeDeclareAsync(
            exchange: "employee.onboarding",
            type: ExchangeType.Topic,
            durable: true,
            cancellationToken: stoppingToken);

        var queue =
            await _channel.QueueDeclareAsync(
                cancellationToken: stoppingToken);

        await _channel.QueueBindAsync(
            queue: queue.QueueName,
            exchange: "employee.onboarding",
            routingKey: "payroll.activated",
            cancellationToken: stoppingToken);

        var consumer = new AsyncEventingBasicConsumer(_channel);

        consumer.ReceivedAsync += async (_, ea) =>
        {
            var @event =
                JsonSerializer.Deserialize<PayrollActivatedIntegrationEvent>(
                    ea.Body.Span);

            if (@event is not null)
            {
                await _sagaService.HandlePayrollActivatedAsync(@event);
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
using System.Text.Json;
using PayrollService.Application.Events;
using PayrollService.Application.Interfaces.Services;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace PayrollService.Api.BackgroundServices;

public class PayrollActivateConsumer : BackgroundService
{
    private readonly IPayrollActivationService _service;
    private IConnection? _connection;
    private IChannel? _channel;

    public PayrollActivateConsumer(IPayrollActivationService service)
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
            "payroll.activate",
            cancellationToken: stoppingToken);

        var consumer = new AsyncEventingBasicConsumer(_channel);

        consumer.ReceivedAsync += async (_, ea) =>
        {
            var @event =
                JsonSerializer.Deserialize<PayrollActivateIntegrationEvent>(
                    ea.Body.Span);

            if (@event != null)
            {
                await _service.ActivateAsync(@event);
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
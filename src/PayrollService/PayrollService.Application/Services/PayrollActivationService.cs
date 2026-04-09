using PayrollService.Application.Events;
using PayrollService.Application.Interfaces.Messaging;
using PayrollService.Application.Interfaces.Repositories;
using PayrollService.Application.Interfaces.Services;
using PayrollService.Domain.Entities;

namespace PayrollService.Application.Services;

public class PayrollActivationService : IPayrollActivationService
{
    private readonly IPayrollRepository _repository;
    private readonly IIntegrationEventPublisher _publisher;

    public PayrollActivationService(
        IPayrollRepository repository,
        IIntegrationEventPublisher publisher)
    {
        _repository = repository;
        _publisher = publisher;
    }

    public async Task ActivateAsync(PayrollActivateIntegrationEvent @event)
    {
        // deterministic failure for even employee IDs to demonstrate failure handling
        if (@event.EmployeeId % 2 == 0)
        {
            var failedEvent = new PayrollActivationFailedIntegrationEvent
            {
                OnboardingProcessId = @event.OnboardingProcessId,
                EmployeeId = @event.EmployeeId,
                Reason = "Payroll validation failed for employee",
                OccurredAtUtc = DateTime.UtcNow
            };

            await _publisher.PublishAsync(
                failedEvent,
                eventName: "payroll.activation.failed");

            return;
        }

        var account = new PayrollAccount
        {
            EmployeeId = @event.EmployeeId,
            SalaryAmount = 100000,
            Currency = "USD",
            IsActive = true,
            ActivatedAtUtc = DateTime.UtcNow
        };

        account = await _repository.CreateAsync(account);

        var activatedEvent = new PayrollActivatedIntegrationEvent
        {
            OnboardingProcessId = @event.OnboardingProcessId,
            EmployeeId = @event.EmployeeId,
            PayrollAccountId = account.PayrollAccountId,
            OccurredAtUtc = DateTime.UtcNow
        };

        await _publisher.PublishAsync(
            activatedEvent,
            eventName: "payroll.activated");
    }
}
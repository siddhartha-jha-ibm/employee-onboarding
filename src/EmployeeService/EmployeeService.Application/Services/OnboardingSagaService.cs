using EmployeeService.Application.Events;
using EmployeeService.Application.Interfaces.Messaging;
using EmployeeService.Application.Interfaces.Repositories;
using EmployeeService.Application.Interfaces.Services;

namespace EmployeeService.Application.Services;

public class OnboardingSagaService : IOnboardingSagaService
{
    private readonly IOnboardingRepository _onboardingRepository;
    private readonly IIntegrationEventPublisher _publisher;

    public OnboardingSagaService(
        IOnboardingRepository onboardingRepository,
        IIntegrationEventPublisher publisher)
    {
        _onboardingRepository = onboardingRepository;
        _publisher = publisher;
    }

    public async Task HandleAccessProvisionedAsync(
        AccessProvisionedIntegrationEvent @event)
    {
        var process =
            await _onboardingRepository.GetByIdAsync(
                @event.OnboardingProcessId);

        if (process == null)
            return;

        process.IsAccessProvisioned = true;
        process.CurrentStep = "AccessProvisioned";
        process.Status = "InProgress";

        await _onboardingRepository.UpdateAsync(process);

        await _onboardingRepository.AddEventAsync(
            new Domain.Entities.OnboardingEvent
            {
                OnboardingProcessId = process.OnboardingProcessId,
                EventType = "AccessProvisioned",
                EventPayload = $"UserAccountId={@event.UserAccountId}",
                OccurredAtUtc = DateTime.UtcNow
            });


        var facilitiesAllocateEvent =
                    new FacilitiesAllocateIntegrationEvent
                    {
                        OnboardingProcessId = process.OnboardingProcessId,
                        EmployeeId = process.EmployeeId,
                        OccurredAtUtc = DateTime.UtcNow
                    };


        await _publisher.PublishAsync(
                  facilitiesAllocateEvent,
                  eventName: "facilities.allocate");

    }

    public async Task HandleFacilitiesAllocatedAsync(
            FacilitiesAllocatedIntegrationEvent @event)
    {
        var process =
            await _onboardingRepository.GetByIdAsync(
                @event.OnboardingProcessId);

        if (process == null)
            return;

        process.IsFacilitiesAssigned = true;
        process.Status = "InProgress";
        process.CurrentStep = "FacilitiesAllocated";

        await _onboardingRepository.UpdateAsync(process);

        await _onboardingRepository.AddEventAsync(
            new Domain.Entities.OnboardingEvent
            {
                OnboardingProcessId = process.OnboardingProcessId,
                EventType = "FacilitiesAllocated",
                EventPayload =
                    $"Desk={@event.DeskCode},Badge={@event.BadgeNumber}",
                OccurredAtUtc = DateTime.UtcNow
            });

        var payrollActivateEvent = new PayrollActivateIntegrationEvent
        {
            OnboardingProcessId = process.OnboardingProcessId,
            EmployeeId = process.EmployeeId,
            OccurredAtUtc = DateTime.UtcNow
        };

        await _publisher.PublishAsync(
            payrollActivateEvent,
            eventName: "payroll.activate");
    }

    public async Task HandlePayrollActivatedAsync(
            PayrollActivatedIntegrationEvent @event)
    {
        var process =
            await _onboardingRepository.GetByIdAsync(
                @event.OnboardingProcessId);

        if (process == null)
            return;

        process.IsPayrollActivated = true;
        process.Status = "Completed";
        process.CurrentStep = "PayrollActivated";
        process.CompletedAtUtc = DateTime.UtcNow;

        await _onboardingRepository.UpdateAsync(process);

        await _onboardingRepository.AddEventAsync(
            new Domain.Entities.OnboardingEvent
            {
                OnboardingProcessId = process.OnboardingProcessId,
                EventType = "OnboardingCompleted",
                EventPayload =
                    $"PayrollAccountId={@event.PayrollAccountId}",
                OccurredAtUtc = DateTime.UtcNow
            });
    }

    public async Task HandlePayrollActivationFailedAsync(
            PayrollActivationFailedIntegrationEvent @event)
    {
        var process =
            await _onboardingRepository.GetByIdAsync(
                @event.OnboardingProcessId);

        if (process == null)
            return;

        process.Status = "Failed";
        process.CurrentStep = "PayrollActivationFailed";
        process.FailureReason = @event.Reason;

        await _onboardingRepository.UpdateAsync(process);

        await _onboardingRepository.AddEventAsync(
            new Domain.Entities.OnboardingEvent
            {
                OnboardingProcessId = process.OnboardingProcessId,
                EventType = "PayrollActivationFailed",
                EventPayload = @event.Reason,
                OccurredAtUtc = DateTime.UtcNow
            });

        //Trigger compensation
        await _publisher.PublishAsync(
            new FacilitiesReleaseIntegrationEvent
            {
                OnboardingProcessId = process.OnboardingProcessId,
                EmployeeId = process.EmployeeId,
                OccurredAtUtc = DateTime.UtcNow
            },
            eventName: "facilities.release");

        await _publisher.PublishAsync(
            new AccessRevokeIntegrationEvent
            {
                OnboardingProcessId = process.OnboardingProcessId,
                EmployeeId = process.EmployeeId,
                OccurredAtUtc = DateTime.UtcNow
            },
            eventName: "access.revoke");
    }

    public async Task HandleFacilitiesReleasedAsync(
        FacilitiesReleasedIntegrationEvent @event)
    {
        var process =
            await _onboardingRepository.GetByIdAsync(
                @event.OnboardingProcessId);

        if (process == null)
            return;

        process.IsFacilitiesAssigned = false;

        await _onboardingRepository.UpdateAsync(process);

        await _onboardingRepository.AddEventAsync(
            new Domain.Entities.OnboardingEvent
            {
                OnboardingProcessId = process.OnboardingProcessId,
                EventType = "FacilitiesReleased",
                EventPayload = "Facilities compensation completed",
                OccurredAtUtc = DateTime.UtcNow
            });

        await FinalizeCompensationIfComplete(process);
    }

    public async Task HandleAccessRevokedAsync(
        AccessRevokedIntegrationEvent @event)
    {
        var process =
            await _onboardingRepository.GetByIdAsync(
                @event.OnboardingProcessId);

        if (process == null)
            return;

        process.IsAccessProvisioned = false;

        await _onboardingRepository.UpdateAsync(process);

        await _onboardingRepository.AddEventAsync(
            new Domain.Entities.OnboardingEvent
            {
                OnboardingProcessId = process.OnboardingProcessId,
                EventType = "AccessRevoked",
                EventPayload = "Access compensation completed",
                OccurredAtUtc = DateTime.UtcNow
            });

        await FinalizeCompensationIfComplete(process);
    }

    private async Task FinalizeCompensationIfComplete(
        Domain.Entities.OnboardingProcess process)
    {
        if (!process.IsAccessProvisioned && !process.IsFacilitiesAssigned)
        {
            process.Status = "Failed";
            process.CurrentStep = "CompensationCompleted";
            process.CompletedAtUtc = DateTime.UtcNow;

            await _onboardingRepository.UpdateAsync(process);

            await _onboardingRepository.AddEventAsync(
                new Domain.Entities.OnboardingEvent
                {
                    OnboardingProcessId = process.OnboardingProcessId,
                    EventType = "OnboardingCompensationCompleted",
                    EventPayload = "Saga fully compensated",
                    OccurredAtUtc = DateTime.UtcNow
                });
        }
    }

}
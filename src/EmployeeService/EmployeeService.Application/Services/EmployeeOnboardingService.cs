using EmployeeService.Application.Contracts;
using EmployeeService.Application.Interfaces.Services;
using EmployeeService.Application.Interfaces.Repositories;
using EmployeeService.Domain.Entities;
using EmployeeService.Application.Interfaces.Messaging;
using EmployeeService.Application.Events;

namespace EmployeeService.Application.Services;

public class EmployeeOnboardingService : IEmployeeOnboardingService
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IOnboardingRepository _onboardingRepository;
    private readonly IIntegrationEventPublisher _eventPublisher;

    public EmployeeOnboardingService(
        IEmployeeRepository employeeRepository,
        IOnboardingRepository onboardingRepository,
        IIntegrationEventPublisher eventPublisher)
    {
        _employeeRepository = employeeRepository;
        _onboardingRepository = onboardingRepository;
        _eventPublisher = eventPublisher;
    }

    public async Task<EmployeeOnboardingResult> StartOnboardingAsync(CreateEmployeeRequest request)
    {
        // 1. Create employee
        var employee = new Employee
        {
            EmployeeCode = request.EmployeeCode,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Department = request.Department,
            JobTitle = request.JobTitle,
            HireDate = request.HireDate,
            CreatedAtUtc = DateTime.UtcNow,
            CreatedBy = request.RequestedBy
        };

        employee = await _employeeRepository.AddAsync(employee);

        // 2. Create onboarding process (Saga start)
        var onboardingProcess = new OnboardingProcess
        {
            OnboardingProcessId = Guid.NewGuid(),
            EmployeeId = employee.EmployeeId,
            Status = "Initiated",
            CurrentStep = "EmployeeCreated",
            StartedAtUtc = DateTime.UtcNow
        };

        await _onboardingRepository.AddAsync(onboardingProcess);

        var integrationEvent = new EmployeeCreatedIntegrationEvent
        {
            OnboardingProcessId = onboardingProcess.OnboardingProcessId,
            EmployeeId = employee.EmployeeId,
            Email = employee.Email,
            Department = employee.Department,
            OccurredAtUtc = DateTime.UtcNow
        };

        await _eventPublisher.PublishAsync(
            integrationEvent,
            eventName: "employee.created");

        // 3. Record onboarding event (audit / trace)
        var onboardingEvent = new OnboardingEvent
        {
            OnboardingProcessId = onboardingProcess.OnboardingProcessId,
            EventType = "EmployeeCreated",
            EventPayload = $"EmployeeId={employee.EmployeeId}",
            OccurredAtUtc = DateTime.UtcNow
        };

        await _onboardingRepository.AddEventAsync(onboardingEvent);

        // 4. Return result
        return new EmployeeOnboardingResult
        {
            EmployeeId = employee.EmployeeId,
            OnboardingProcessId = onboardingProcess.OnboardingProcessId,
            Status = onboardingProcess.Status
        };
    }
}
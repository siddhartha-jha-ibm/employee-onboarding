using PayrollService.Application.Events;

namespace PayrollService.Application.Interfaces.Services;

public interface IPayrollActivationService
{
    Task ActivateAsync(PayrollActivateIntegrationEvent @event);
}

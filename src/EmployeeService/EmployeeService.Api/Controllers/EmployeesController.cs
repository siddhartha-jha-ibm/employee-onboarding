using EmployeeService.Application.Contracts;
using EmployeeService.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeService.Api.Controllers;

[ApiController]
[Route("api/employees")]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeOnboardingService _onboardingService;

    public EmployeesController(IEmployeeOnboardingService onboardingService)
    {
        _onboardingService = onboardingService;
    }

    /// <summary>
    /// Starts the employee onboarding process.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateEmployee(
        [FromBody] CreateEmployeeRequest request)
    {
        var result = await _onboardingService.StartOnboardingAsync(request);

        return CreatedAtAction(
            nameof(GetEmployeeOnboardingStatus),
            new { onboardingProcessId = result.OnboardingProcessId },
            result);
    }

    /// <summary>
    /// Gets Employee Onboarding Status
    /// </summary>
    [HttpGet("onboarding/{onboardingProcessId}")]
    public async Task<IActionResult> GetEmployeeOnboardingStatus(
        Guid onboardingProcessId,
        [FromServices] IOnboardingStatusQueryService queryService)
    {
        var status = await queryService.GetStatusAsync(onboardingProcessId);

        if (status == null)
            return NotFound();

        return Ok(status);
    }
}
using EmployeeService.Api.BackgroundServices;
using EmployeeService.Application.DependencyInjection;
using EmployeeService.Infrastructure.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();

// Application & Infrastructure
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// Hosted services (RabbitMQ consumers)
builder.Services.AddHostedService<AccessProvisionedConsumer>();
builder.Services.AddHostedService<FacilitiesAllocatedConsumer>();
builder.Services.AddHostedService<PayrollActivatedConsumer>();
builder.Services.AddHostedService<PayrollActivationFailedConsumer>();
builder.Services.AddHostedService<FacilitiesReleasedConsumer>();
builder.Services.AddHostedService<AccessRevokedConsumer>();

// Adds the built-in OpenAPI services (replaces Swashbuckle/SwaggerGen)
builder.Services.AddOpenApi(); 

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

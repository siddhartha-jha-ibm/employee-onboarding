using PayrollService.Application.DependencyInjection;
using PayrollService.Infrastructure.DependencyInjection;
using PayrollService.Api.BackgroundServices;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddHostedService<PayrollActivateConsumer>();

var app = builder.Build();
app.Run();
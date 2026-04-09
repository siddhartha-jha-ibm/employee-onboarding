using IdentityService.Application.DependencyInjection;
using IdentityService.Infrastructure.DependencyInjection;
using IdentityService.Api.BackgroundServices;

var builder = WebApplication.CreateBuilder(args);

// Application & Infrastructure
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// Hosted services (RabbitMQ consumers)
builder.Services.AddHostedService<EmployeeCreatedConsumer>();
builder.Services.AddHostedService<AccessRevokeConsumer>();

// Minimal API plumbing
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

app.Run();
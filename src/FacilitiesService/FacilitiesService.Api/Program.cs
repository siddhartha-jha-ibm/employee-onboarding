using FacilitiesService.Application.DependencyInjection;
using FacilitiesService.Infrastructure.DependencyInjection;
using FacilitiesService.Api.BackgroundServices;

var builder = WebApplication.CreateBuilder(args);

// Application & Infrastructure
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// RabbitMQ Consumers
builder.Services.AddHostedService<FacilitiesAllocateConsumer>();
builder.Services.AddHostedService<FacilitiesReleaseConsumer>();

// Minimal API plumbing (no controllers for now)
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

app.Run();
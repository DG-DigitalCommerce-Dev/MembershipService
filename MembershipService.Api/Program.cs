using MembershipService.Application.Common.Interfaces;
using MembershipService.Application.Services;
using MembershipService.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add controllers
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Subscription Services ONLY
builder.Services.AddScoped<ISubscriptionService, SubscriptionService>();

// Infrastructure (registers IVtexClient, VtexClient, VtexSettings)
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();

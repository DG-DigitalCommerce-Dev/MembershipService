using AutoMapper;
using MembershipService.Application.Common.Interfaces;
using MembershipService.Application.Mapping;
using MembershipService.Application.Services;
using MembershipService.Api.Mappings;
using MembershipService.Infrastructure.Extensions;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAutoMapper(typeof(DomainToDtoProfile).Assembly);
builder.Services.AddAutoMapper(typeof(SubscriptionProfile).Assembly);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<ISubscriptionService, SubscriptionService>();
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

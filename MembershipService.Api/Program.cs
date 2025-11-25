using AutoMapper;
using MembershipService.Api.Mappings;
using MembershipService.Application.Common.Interfaces;
using MembershipService.Application.Common.Mappings;
using MembershipService.Application.Mapping;
using MembershipService.Application.Services;
using MembershipService.Domain.Models;
using MembershipService.Infrastructure.Extensions.MembershipService.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAutoMapper(typeof(DomainToDtoProfile).Assembly);
builder.Services.AddAutoMapper(typeof(SubscriptionProfile).Assembly);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient();
builder.Services.AddScoped<IMembershipDataService, MembershipDataService>();
builder.Services.AddAutoMapper(typeof(DtoToResponseProfile), typeof(MembershipDataToDtoProfile));
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddScoped<ISubscriptionService, SubscriptionService>();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.Configure<VtexApiSettings>(builder.Configuration.GetSection("VtexApi"));
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

using AutoMapper;
using MembershipService.Application.Common.Interfaces;
using MembershipService.Application.Services;
using MembershipService.Infrastructure.Integrations;
using MembershipService.Infrastructure.Interfaces;
using MembershipService.Application.Common.Mappings;
using MembershipService.Infrastructure.Extensions;
using MembershipService.Api.Mapping;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient();
builder.Services.AddScoped<IMembershipDataService, MembershipDataService>();
builder.Services.AddAutoMapper(typeof(DtoToResponseProfile),typeof(MembershipDataToDtoProfile));
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


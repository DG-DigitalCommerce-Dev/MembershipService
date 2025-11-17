using AutoMapper;
using MembershipService.Application.Common.Interfaces;
using MembershipService.Application.Services;
using MembershipService.Infrastructure.Integrations;
using MembershipService.Infrastructure.Interfaces;
using MembershipService.Application.Common.Mappings;
using MembershipService.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddHttpClient();
builder.Services.AddScoped<IMembershipDataService, MembershipDataService>();
builder.Services.AddAutoMapper(typeof(MembershipProfile).Assembly);
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();


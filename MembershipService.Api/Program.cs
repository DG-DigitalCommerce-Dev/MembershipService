using MembershipService.Application.Services;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;

builder.Services.AddScoped<ISubscriptionService, SubscriptionService>();

var builder = WebApplication.CreateBuilder(args);
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

// 1.Add services to the container.
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins, policy =>
    {
        policy.SetIsOriginAllowed(origin =>
        {
            if (!Uri.TryCreate(origin, UriKind.Absolute, out var uri))
                return false;

            return uri.Scheme == Uri.UriSchemeHttps &&
                   uri.Host.EndsWith(".dollargeneral.com", StringComparison.OrdinalIgnoreCase);
        });
    });
});

// 2? Adding controllers and configuring JSON serialization
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});

// 3? Configuring HttpClient for SubscriptionService (Catalog + Pricing APIs)
builder.Services.AddHttpClient<SubscriptionService>()
    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
    {
        // This bypasses SSL certificate validation in dev environment
        ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
    });

// 4? Adding Swagger/OpenAPI support
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Subscription API",
        Version = "v1",
        Description = "API for fetching VTEX product and pricing data."
    });

    var xmlFilename = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
    if (File.Exists(xmlPath))
        c.IncludeXmlComments(xmlPath);
});

var app = builder.Build();

// 5. Configure the HTTP request pipeline.
if (!app.Environment.IsProduction())
{
    var pathBase = builder.Configuration["basePath"];
    if (!string.IsNullOrEmpty(pathBase))
    {
        app.UsePathBase(pathBase);
    }

    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        var swaggerEndpoint = $"{(!string.IsNullOrEmpty(pathBase) ? pathBase : string.Empty)}/swagger/v1/swagger.json";
        c.SwaggerEndpoint(swaggerEndpoint, "Subscription API V1");
    });
}

app.UseCors(MyAllowSpecificOrigins);

app.UseAuthorization();

app.MapControllers();

app.Run();

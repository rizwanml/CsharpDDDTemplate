using Asp.Versioning;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Formatting.Compact;
using SmallService.API.Filters;
using SmallService.API.Options;
using SmallService.Domain.Configuration;
using SmallService.Infrastructure.Configuration;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

WebApplicationBuilder builder;

var configuration = new ConfigurationBuilder()
                       .SetBasePath(Directory.GetCurrentDirectory())
                       .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                       .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", true)
                       .AddEnvironmentVariables()
                       .AddUserSecrets(Assembly.GetExecutingAssembly(), true)
                       .Build();

//Environment in Visual Studio and Docker Compose are set to local in order to develop and test with local configurations or services ie: Localstack
//NOTE: This bool can be passed around when setting up the IoC Container where local specific setup is required
bool isLocalEnvironment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Local" ? true : false;

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .Enrich.WithProperty("ApplicationName", "SmallService.API")
    .WriteTo.Console(new RenderedCompactJsonFormatter())
    .ReadFrom.Configuration(configuration)
    .CreateBootstrapLogger();

builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(c =>
{
    c.AddPolicy("AllowOrigin", options => {
        options.AllowAnyOrigin();
        options.AllowAnyHeader();
    });
});

builder.Services.AddHealthChecks();

builder.Services.Configure<CircuitBreakerResponseOptions>(configuration.GetSection(nameof(CircuitBreakerResponseOptions))).AddOptions<CircuitBreakerResponseOptions>();
builder.Services.TryAddSingleton<CircuitBreakerResponseFilter>();

builder.Services.AddControllers(options =>
{
    options.Filters.AddService<CircuitBreakerResponseFilter>();
});

builder.Services.AddApiVersioning(options =>
{
    options.AssumeDefaultVersionWhenUnspecified = false;
    options.ReportApiVersions = true;
    options.ApiVersionReader = new UrlSegmentApiVersionReader(); //adjust for version reading options
}).AddMvc().AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

builder.Services.AddSwaggerGen(options =>
{
    options.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());

    //Define Swagger Documents
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "SmallService", Version = "v1", Description = "Description" });
    options.SwaggerDoc("v2", new OpenApiInfo { Title = "SmallService", Version = "v2", Description = "Description" });

    //Apply Swashbuckle filters to aid in swagger generation

    //Ensure the routes are added to the correct Swagger document
    options.DocInclusionPredicate((version, desc) =>
    {
        if (!desc.TryGetMethodInfo(out MethodInfo methodInfo))
        {
            return false;
        }

        var versions = methodInfo.DeclaringType
            .GetCustomAttributes(true)
            .OfType<ApiVersionAttribute>()
            .SelectMany(attr => attr.Versions);

        var maps = methodInfo
            .GetCustomAttributes(true)
            .OfType<MapToApiVersionAttribute>()
            .SelectMany(attr => attr.Versions)
            .ToArray();

        return versions.Any(v => $"v{v.ToString()}" == version)
        && (!maps.Any() || maps.Any(v => $"v{v.ToString()}" == version));
    });
});

builder.Services.AddInfrastructureServices(configuration, isLocalEnvironment);
builder.Services.AddDomainServices(configuration);

builder.Host.UseSerilog();

// Use to enable kafka event streaming
//builder.Services.KafkaFlowSetup(configuration);
//builder.Services.KafkaFlowStartConsuming(configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseCors(options =>
{
    options.AllowAnyOrigin();
    options.AllowAnyMethod();
    options.AllowAnyHeader();
});

app.UseHttpsRedirection();

app.UseSerilogRequestLogging();

app.MapHealthChecks("/health");

app.UseAuthorization();

app.MapControllers();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "SmallService V1");
    c.SwaggerEndpoint("/swagger/v2/swagger.json", "SmallService V2");
    c.RoutePrefix = string.Empty;
});

try
{
    Log.Information("Starting SmallService API");
    app.Run();
}
catch (Exception e)
{
    Log.Fatal(e, "Unable to start SmallService API");
}
finally
{
    Log.CloseAndFlush();
}

public partial class Program { }
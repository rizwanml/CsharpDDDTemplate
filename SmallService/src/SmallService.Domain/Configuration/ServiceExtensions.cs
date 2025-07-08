using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmallService.Domain.Options;
using SmallService.Domain.Services.ExamplePersonModule;

namespace SmallService.Domain.Configuration;

public static partial class ServiceExtensions
{
    public static IServiceCollection AddDomainServices(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection.AddTransient<IExamplePersonService, ExamplePersonService>();
        serviceCollection.Configure<ExampleDomainOptions>(configuration.GetSection(nameof(ExampleDomainOptions))).AddOptions<ExampleDomainOptions>();

        return serviceCollection;
    }
}
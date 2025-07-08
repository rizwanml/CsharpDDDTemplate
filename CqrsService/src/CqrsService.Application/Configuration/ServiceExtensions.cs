using AutoMapper.Extensions.ExpressionMapping;
using Microsoft.Extensions.DependencyInjection;

namespace CqrsService.Application.Configuration;

public static partial class ServiceExtensions
{
    public static IServiceCollection AddApplcationServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddMediator(options =>
        {
            options.ServiceLifetime = ServiceLifetime.Scoped;
        });

        serviceCollection.AddAutoMapper(cfg => { cfg.AddProfile<ApplicationAutoMapperProfile>(); cfg.AddExpressionMapping(); });

        return serviceCollection;
    }
}
using SmallService.Domain.Configuration.Framework;

namespace SmallService.Domain.InfrastructureContracts.Processors;

public interface IExampleProcessor
{
    Task<TModel> SendRequestExample<TModel>(TModel model) where TModel : DomainEntity;
}
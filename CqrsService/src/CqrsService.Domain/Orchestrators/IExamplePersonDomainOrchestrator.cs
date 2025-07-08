using CqrsService.Domain.Configuration.Framework;
using CqrsService.Shared;
using System.Linq.Expressions;

namespace CqrsService.Domain.Orchestrators;

/// <summary>
/// The infrastrucure orchestrator that implements the default command operations and extends the orchestration operations where needed
/// </summary>
public interface IExamplePersonDomainOrchestrator
{
    Task<TModel> Create<TModel>(TModel model) where TModel : DomainEntity;
    Task<TModel> GetPersonWithParams<TModel>(params object[] parameters) where TModel : DomainEntity;
    Task<TModel> GetValueObject<TModel>(Expression<Func<TModel, bool>> expression) where TModel : DomainValueObject;
    Task<TModel> GetById<TModel>(Expression<Func<TModel, bool>> expression) where TModel : DomainEntity;
    Task<IEnumerable<TModel>> GetAll<TModel>(Expression<Func<TModel, bool>> expression) where TModel : DomainEntity;
    Task<TModel> Update<TModel>(Expression<Func<TModel, bool>> expression, TModel model) where TModel : DomainEntity;
    Task<bool> Delete<TModel>(Expression<Func<TModel, bool>> expression) where TModel : DomainEntity;
}
using System.Linq.Expressions;
using SmallService.Domain.Configuration.Framework;

namespace SmallService.Domain.InfrastructureContracts.Repositories;

public interface IExamplePersonRepository
{
    Task<TModel> Create<TModel>(TModel model) where TModel : DomainEntity;
    Task<TModel> GetPersonWithParams<TModel>(params object[] parameters) where TModel : DomainEntity;
    Task<TModel> GetById<TModel>(Expression<Func<TModel, bool>> expression) where TModel : DomainEntity;
    Task<IEnumerable<TModel>> GetAll<TModel>(Expression<Func<TModel, bool>> expression) where TModel : DomainEntity;
    Task<TModel> Update<TModel>(Expression<Func<TModel, bool>> expression, TModel model) where TModel : DomainEntity;
    Task<bool> Delete<TModel>(Expression<Func<TModel, bool>> expression) where TModel : DomainEntity;
}
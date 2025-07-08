using System.Linq.Expressions;
using AutoMapper;
using AutoMapper.Extensions.ExpressionMapping;
using SmallService.Domain.Configuration.Framework;
using SmallService.Domain.Entities.ExamplePersonModule;
using SmallService.Domain.InfrastructureContracts.Repositories;
using SmallService.Infrastructure.Abstractions.Persistence.InMemory;
using SmallService.Infrastructure.Providers.ReqRes.Models;
using SmallService.Infrastructure.Repositories.Entities;

namespace SmallService.Infrastructure.Repositories;

public sealed class ExamplePersonRepository : IExamplePersonRepository
{
    private readonly IMapper _mapper;
    private readonly IInMemory _inMemory;

    public ExamplePersonRepository(IMapper mapper,
        IInMemory inMemory)
    {
        _mapper = mapper;
        _inMemory = inMemory;
    }

    public async Task<TModel> Create<TModel>(TModel model) where TModel : DomainEntity
    {
        //An example of creating a concrete type from the generic domain model if you need to access it's properties
        ExamplePerson examplePerson = (dynamic)model;

        //An example of mapping the domain entity to the persistence entity
        var persistenceEntityMap = _mapper.Map<ExamplePersonDbEntity>(model);

        //An example of creating the persistence entity from the domain entity
        var createdPersistenceEntity = new ExamplePersonDbEntity(examplePerson.Id,
            examplePerson.FirstName,
            examplePerson.LastName,
            examplePerson.Age);

        //An example of calling the persistence with the persistence entity
        ExamplePersonDbEntity persistenceResponse = await _inMemory.Create(persistenceEntityMap);

        //An example of creating the provider model from the domain entity
        var createdExternalProviderModel = new CreateExamplePersonRequest(examplePerson.FirstName, examplePerson.LastName);
        var createdQueueProviderModel = new ExamplePersonModel(examplePerson.Id,
            examplePerson.FirstName,
            examplePerson.LastName,
            examplePerson.Age);

        //Returning a successful content
        return model;
    }

    public async Task<TModel> GetById<TModel>(Expression<Func<TModel, bool>> expression) where TModel : DomainEntity
    {
        var persistenceEntityExpressionMap = _mapper.MapExpression<Expression<Func<ExamplePersonDbEntity, bool>>>(expression);

        ExamplePersonDbEntity persistenceResponse =
            await _inMemory.QuerySingle(persistenceEntityExpressionMap);

        var domainEntityMap = _mapper.Map<ExamplePerson>(persistenceResponse) as TModel;

        return domainEntityMap;
    }

    public async Task<IEnumerable<TModel>> GetAll<TModel>(Expression<Func<TModel, bool>> expression) where TModel : DomainEntity
    {
        var persistenceEntityExpressionMap = _mapper.Map<Expression<Func<ExamplePersonDbEntity, bool>>>(expression);

        IEnumerable<ExamplePersonDbEntity> persistenceResponse =
            await _inMemory.QueryMultiple(persistenceEntityExpressionMap);

        return _mapper.Map<IEnumerable<TModel>>(persistenceResponse);
    }

    public async Task<TModel> GetPersonWithParams<TModel>(params object[] parameters) where TModel : DomainEntity
    {
        //This is just an example of how to use the params object in the request
        //This could be used for things like calling stored procedures etc.
        var firstName = parameters[0];
        var lastName = parameters[1];
        Expression<Func<ExamplePersonDbEntity, bool>> expression = p => p.FirstName == firstName.ToString() && p.LastName == lastName.ToString();

        ExamplePersonDbEntity persistenceResponse = await _inMemory.QuerySingle(expression);

        return _mapper.Map<TModel>(persistenceResponse);
    }

    public async Task<TModel> Update<TModel>(Expression<Func<TModel, bool>> expression, TModel model) where TModel : DomainEntity
    {
        var persistenceEntityMap = _mapper.Map<ExamplePersonDbEntity>(model);

        var persistenceEntityExpressionMap = _mapper.Map<Expression<Func<ExamplePersonDbEntity, bool>>>(expression);

        ExamplePersonDbEntity persistenceResponse =
            await _inMemory.Update(persistenceEntityExpressionMap, persistenceEntityMap);

        var domainEntityMap = _mapper.Map<ExamplePerson>(persistenceResponse) as TModel;

        return domainEntityMap;
    }

    public async Task<bool> Delete<TModel>(Expression<Func<TModel, bool>> expression) where TModel : DomainEntity
    {
        var persistenceEntityExpressionMap = _mapper.Map<Expression<Func<ExamplePersonDbEntity, bool>>>(expression);

        bool persistenceResponse = await _inMemory.Delete(persistenceEntityExpressionMap);

        return persistenceResponse;
    }
}
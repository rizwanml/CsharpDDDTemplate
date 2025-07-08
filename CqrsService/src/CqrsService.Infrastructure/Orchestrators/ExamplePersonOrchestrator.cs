using System.Linq.Expressions;
using AutoMapper;
using AutoMapper.Extensions.ExpressionMapping;
using CqrsService.Domain.Configuration.Framework;
using CqrsService.Domain.Entities.ExamplePersonModule;
using CqrsService.Domain.Orchestrators;
using CqrsService.Infrastructure.Persistence.Entities;
using CqrsService.Infrastructure.Persistence.InMemory;
using CqrsService.Infrastructure.Provider.External.ReqRes;
using CqrsService.Infrastructure.Provider.External.ReqRes.Models;
using CqrsService.Infrastructure.Provider.Messaging.Kafka;
using CqrsService.Infrastructure.Provider.Messaging.Kafka.Models;

namespace CqrsService.Infrastructure.Orchestrators;

/// <summary>
/// The infrastructure orchestrator that encapsulates the orchestration between the infrastructure implementation required to complete the task.
/// For example: if the task requires a call to an external provider and an interaction with a persistent data storage the orchestration would go here.
/// </summary>
public sealed class ExamplePersonOrchestrator : IExamplePersonDomainOrchestrator
{
    private readonly IMapper _mapper;
    private readonly IInMemoryPersistence _inMemoryPersistence;
    private readonly IReqResProvider _reqResProvider;

    public ExamplePersonOrchestrator(IMapper mapper,
        IInMemoryPersistence inMemoryPersistence,
        IReqResProvider reqResProvider)
    {
        _mapper = mapper;
        _inMemoryPersistence = inMemoryPersistence;
        _reqResProvider = reqResProvider;
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
        var persistenceResponse = await _inMemoryPersistence.Create(persistenceEntityMap);

        //An example of mapping the domain entity to a provider model
        var externalProviderModelMap = _mapper.Map<CreateExamplePersonRequest>(model);
        var queueProviderModelMap = _mapper.Map<ExamplePersonModel>(model);

        //An example of creating the provider model from the domain entity
        var createdExternalProviderModel = new CreateExamplePersonRequest(examplePerson.FirstName, examplePerson.LastName);
        var createdQueueProviderModel = new ExamplePersonModel(examplePerson.Id,
            examplePerson.FirstName,
            examplePerson.LastName,
            examplePerson.Age);

        //Various examples of calling an external provider with various request/response types
        var createUserWithGenericRequestResponse = await _reqResProvider.CreateUserWithGenericRequestResponse(externalProviderModelMap);
        var createUserWithRequestResponseModel = await _reqResProvider.CreateUserWithRequestResponseModel(externalProviderModelMap);
        var createUserWithGenericRequest = await _reqResProvider.CreateUserWithGenericRequest(externalProviderModelMap);
        var createUserWithGenericResponse = await _reqResProvider.CreateUserWithGenericResponse<CreateExamplePersonResponse>(examplePerson);

        //Returning a successful content
        return model;
    }

    public async Task<TModel> GetValueObject<TModel>(Expression<Func<TModel, bool>> expression) where TModel : DomainValueObject
    {
        var persistenceEntityExpressionMap = _mapper.MapExpression<Expression<Func<ExamplePersonDbEntity, bool>>>(expression);

        var persistenceResponse = await _inMemoryPersistence.QuerySingle(persistenceEntityExpressionMap);

        var valueObject = new Domain.ValueObjects.ExamplePersonModule.ExamplePerson("firsname", "lastname", 20) as TModel;

        return valueObject;
    }

    public async Task<TModel> GetById<TModel>(Expression<Func<TModel, bool>> expression) where TModel : DomainEntity
    {
        var persistenceEntityExpressionMap = _mapper.MapExpression<Expression<Func<ExamplePersonDbEntity, bool>>>(expression);

        var persistenceResponse = await _inMemoryPersistence.QuerySingle(persistenceEntityExpressionMap);

        var domainEntityMap = _mapper.Map<ExamplePerson>(persistenceResponse) as TModel;

        return domainEntityMap;
    }

    public async Task<IEnumerable<TModel>> GetAll<TModel>(Expression<Func<TModel, bool>> expression) where TModel : DomainEntity
    {
        var persistenceEntityExpressionMap = _mapper.Map<Expression<Func<ExamplePersonDbEntity, bool>>>(expression);

        var persistenceResponse = await _inMemoryPersistence.QueryMultiple(persistenceEntityExpressionMap);


        var domainEntityMap = _mapper.Map<IEnumerable<ExamplePerson>>(persistenceResponse) as IEnumerable<TModel>;

        return domainEntityMap;
    }

    public async Task<TModel> GetPersonWithParams<TModel>(params object[] parameters) where TModel : DomainEntity
    {
        //This is just an example of how to use the params object in the request
        //This could be used for things like calling stored procedures etc.
        var firstName = parameters[0];
        var lastName = parameters[1];
        Expression<Func<ExamplePersonDbEntity, bool>> expression = p => p.FirstName == firstName.ToString() && p.LastName == lastName.ToString();

        var persistenceResponse = await _inMemoryPersistence.QuerySingle(expression);

        var domainEntityMap = _mapper.Map<ExamplePerson>(persistenceResponse) as TModel;

        return domainEntityMap;
    }

    public async Task<TModel> Update<TModel>(Expression<Func<TModel, bool>> expression, TModel model) where TModel : DomainEntity
    {
        var persistenceEntityMap = _mapper.Map<ExamplePersonDbEntity>(model);

        var persistenceEntityExpressionMap = _mapper.Map<Expression<Func<ExamplePersonDbEntity, bool>>>(expression);

        var persistenceResponse = await _inMemoryPersistence.Update(persistenceEntityExpressionMap, persistenceEntityMap);

        var domainEntityMap = _mapper.Map<ExamplePerson>(persistenceResponse) as TModel;

        return domainEntityMap;
    }

    public async Task<bool> Delete<TModel>(Expression<Func<TModel, bool>> expression) where TModel : DomainEntity
    {
        var persistenceEntityExpressionMap = _mapper.Map<Expression<Func<ExamplePersonDbEntity, bool>>>(expression);

        var persistenceResponse = await _inMemoryPersistence.Delete(persistenceEntityExpressionMap);

        return persistenceResponse;
    }
}
using AutoMapper;
using SmallService.Domain.Configuration.Framework;
using SmallService.Domain.Entities.ExamplePersonModule;
using SmallService.Domain.InfrastructureContracts.Processors;
using SmallService.Infrastructure.Providers.ReqRes;
using SmallService.Infrastructure.Providers.ReqRes.Models;

namespace SmallService.Infrastructure.Processors;

public sealed class ExampleProcessor : IExampleProcessor
{
    private readonly IReqResProvider _reqResProvider;
    private readonly IMapper _mapper;

    public ExampleProcessor(IReqResProvider reqResProvider,
        IMapper mapper)
    {
        _reqResProvider = reqResProvider;
        _mapper = mapper;
    }

    public async Task<TModel> SendRequestExample<TModel>(TModel model) where TModel : DomainEntity
    {
        //An example of mapping the domain entity to a provider model
        CreateExamplePersonRequest? externalProviderModelMap = _mapper.Map<CreateExamplePersonRequest>(model);

        //An example of creating a concrete type from the generic domain model if you need to access its properties
        ExamplePerson examplePerson = (dynamic)model;

        //Various examples of calling an external provider with various request/response types
        CreateExamplePersonRequest createUserWithGenericRequestResponse =
            await _reqResProvider.CreateUserWithGenericRequestResponse(externalProviderModelMap);
        CreateExamplePersonResponse createUserWithRequestResponseModel =
            await _reqResProvider.CreateUserWithRequestResponseModel(externalProviderModelMap);
        CreateExamplePersonResponse createUserWithGenericRequest =
            await _reqResProvider.CreateUserWithGenericRequest(externalProviderModelMap);
        CreateExamplePersonResponse createUserWithGenericResponse =
            await _reqResProvider.CreateUserWithGenericResponse<CreateExamplePersonResponse>(examplePerson);

        return model;
    }
}
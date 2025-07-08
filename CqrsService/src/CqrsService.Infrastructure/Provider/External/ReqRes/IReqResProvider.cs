using CqrsService.Domain.Entities.ExamplePersonModule;
using CqrsService.Infrastructure.Provider.External.ReqRes.Models;
using CqrsService.Shared;

namespace CqrsService.Infrastructure.Provider.External.ReqRes;

public interface IReqResProvider
{
    Task<TModel> CreateUserWithGenericRequestResponse<TModel>(TModel model) where TModel : class;
    Task<CreateExamplePersonResponse> CreateUserWithRequestResponseModel(CreateExamplePersonRequest examplePerson);
    Task<CreateExamplePersonResponse> CreateUserWithGenericRequest<TModel>(TModel model) where TModel : class;
    Task<TModel> CreateUserWithGenericResponse<TModel>(ExamplePerson examplePerson) where TModel : class;
}
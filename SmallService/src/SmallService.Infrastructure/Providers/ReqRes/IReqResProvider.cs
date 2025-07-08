using SmallService.Domain.Entities.ExamplePersonModule;
using SmallService.Infrastructure.Providers.ReqRes.Models;

namespace SmallService.Infrastructure.Providers.ReqRes;

public interface IReqResProvider
{
    Task<TModel> CreateUserWithGenericRequestResponse<TModel>(TModel model) where TModel : class;
    Task<CreateExamplePersonResponse> CreateUserWithRequestResponseModel(CreateExamplePersonRequest examplePerson);
    Task<CreateExamplePersonResponse> CreateUserWithGenericRequest<TModel>(TModel model) where TModel : class;
    Task<TModel> CreateUserWithGenericResponse<TModel>(ExamplePerson examplePerson) where TModel : class;
}
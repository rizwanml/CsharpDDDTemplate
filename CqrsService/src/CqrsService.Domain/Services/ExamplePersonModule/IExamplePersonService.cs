using CqrsService.Domain.Configuration.Framework;
using CqrsService.Domain.Entities.ExamplePersonModule;

namespace CqrsService.Domain.Services.ExamplePersonModule;

public interface IExamplePersonService
{
    Task<Response<ExamplePerson>> CreatePerson(ExamplePerson model);
    Task<Response<ExamplePerson>> UpdatePerson(ExamplePerson model);
    Task<Response<bool>> DeletePerson(string id);
    Task<Response<bool>> CircuitBreakerResponse();
}
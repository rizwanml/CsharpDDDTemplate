using SmallService.Domain.Configuration.Framework;
using SmallService.Domain.Entities.ExamplePersonModule;

namespace SmallService.Domain.Services.ExamplePersonModule;

public interface IExamplePersonService
{
    Task<Response<ExamplePerson>> CreatePerson(ExamplePerson examplePerson);
    Task<Response<bool>> DeletePerson(string id);
    Task<Response<IEnumerable<ExamplePerson>>> GetAllPersons();
    Task<Response<ExamplePerson>> GetPersonById(string id);
    Task<Response<ExamplePerson>> GetPersonWithParams(string firstName, string lastName);
    Task<Response<ExamplePerson>> UpdatePerson(ExamplePerson examplePerson);
    Task<Response<bool>> CircuitBreakerResponse();
}
using SmallService.Domain.Configuration;
using SmallService.Domain.Configuration.Framework;
using SmallService.Domain.Entities.ExamplePersonModule;
using SmallService.Domain.ErrorResponses;
using SmallService.Domain.InfrastructureContracts.Processors;
using SmallService.Domain.InfrastructureContracts.Repositories;
using SmallService.Shared;

namespace SmallService.Domain.Services.ExamplePersonModule;

/// <summary>
/// The domain service encapsulates the business rules and logic
/// The domain service calls out to one or more infrastructure orchestrators to build it's domain entities
/// </summary>
public sealed class ExamplePersonService : IExamplePersonService
{
    private readonly IExamplePersonRepository _examplePersonRepository;
    private readonly IExampleProcessor _exampleProcessor;

    public ExamplePersonService(IExamplePersonRepository examplePersonRepository,
        IExampleProcessor exampleProcessor)
    {
        _examplePersonRepository = examplePersonRepository;
        _exampleProcessor = exampleProcessor;
    }

    public async Task<Response<ExamplePerson>> CreatePerson(ExamplePerson examplePerson)
    {
        try
        {
            if (!examplePerson.IsValid())
            {
                return Response<ExamplePerson>.Failure(new DomainValidationErrorResponse(examplePerson, examplePerson.GetValidationErrors()));
            }

            //Example of returning a non domain entity validation error response
            if (examplePerson is null)
            {
                return Response<ExamplePerson>.Failure(new DomainValidationErrorResponse(examplePerson, nameof(examplePerson.FirstName), MessageContext.ErrorExample) );
            }

            //Example of calling a infrastructure processor to process external requests or send messages
            _exampleProcessor.SendRequestExample(examplePerson);

            ExamplePerson response = await _examplePersonRepository.Create(examplePerson);

            return Response<ExamplePerson>.Success(response);
        }
        catch (Exception ex)
        {
            ExceptionHandlerHelpers.HandleException(exception: ex, className: nameof(ExamplePersonService), methodName: nameof(CreatePerson));
            return Response<ExamplePerson>.Failure(new SystemErrorResponse(ex));
        }
    }

    public async Task<Response<ExamplePerson>> GetPersonById(string id)
    {
        try
        {
            ExamplePerson response = await _examplePersonRepository.GetById<ExamplePerson>(p => p.Id == id);

            return Response<ExamplePerson>.Success(response);
        }
        catch (Exception ex)
        {
            ExceptionHandlerHelpers.HandleException(exception: ex, className: nameof(ExamplePersonService), methodName: nameof(GetPersonById));
            return Response<ExamplePerson>.Failure(new SystemErrorResponse(ex));
        }
    }

    public async Task<Response<IEnumerable<ExamplePerson>>> GetAllPersons()
    {
        try
        {
            IEnumerable<ExamplePerson> response = await _examplePersonRepository.GetAll<ExamplePerson>(p => p.Id != null);

            return Response<IEnumerable<ExamplePerson>>.Success(response);
        }
        catch (Exception ex)
        {
            ExceptionHandlerHelpers.HandleException(exception: ex, className: nameof(ExamplePersonService), methodName: nameof(GetAllPersons));
            return Response<IEnumerable<ExamplePerson>>.Failure(new SystemErrorResponse(ex));
        }
    }

    public async Task<Response<ExamplePerson>> GetPersonWithParams(string firstName, string lastName)
    {
        try
        {
            ExamplePerson response = await _examplePersonRepository.GetPersonWithParams<ExamplePerson>(firstName, lastName);

            return Response<ExamplePerson>.Success(response);
        }
        catch (Exception ex)
        {
            ExceptionHandlerHelpers.HandleException(exception: ex, className: nameof(ExamplePersonService), methodName: nameof(GetPersonWithParams));
            return Response<ExamplePerson>.Failure(new SystemErrorResponse(ex));
        }
    }

    public async Task<Response<ExamplePerson>> UpdatePerson(ExamplePerson examplePerson)
    {
        try
        {
            if (!examplePerson.IsValid())
            {
                return Response<ExamplePerson>.Failure(new DomainValidationErrorResponse(examplePerson, examplePerson.GetValidationErrors()));
            }

            ExamplePerson response = await _examplePersonRepository.Update(p => p.Id == examplePerson.Id, examplePerson);

            return Response<ExamplePerson>.Success(response);
        }
        catch (Exception ex)
        {
            ExceptionHandlerHelpers.HandleException(exception: ex, className: nameof(ExamplePersonService), methodName: nameof(UpdatePerson));
            return Response<ExamplePerson>.Failure(new SystemErrorResponse(ex));
        }
    }

    public async Task<Response<bool>> DeletePerson(string id)
    {
        try
        {
            bool response = await _examplePersonRepository.Delete<ExamplePerson>(p => p.Id == id);

            return Response<bool>.Success(response);
        }
        catch (Exception ex)
        {
            ExceptionHandlerHelpers.HandleException(exception: ex, className: nameof(ExamplePersonService), methodName: nameof(DeletePerson));
            return Response<bool>.Failure(new SystemErrorResponse(ex));
        }
    }

    public async Task<Response<bool>> CircuitBreakerResponse()
    {
        try
        {
            throw new NotImplementedException();
        }
        catch (Exception ex)
        {
            ExceptionHandlerHelpers.HandleException(exception: ex, className: nameof(ExamplePersonService), methodName: nameof(CircuitBreakerResponse));
            return Response<bool>.Failure(new SystemErrorResponse(ex));
        }
    }
}
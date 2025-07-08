using CqrsService.Domain.Configuration;
using CqrsService.Domain.Configuration.Framework;
using CqrsService.Domain.Entities.ExamplePersonModule;
using CqrsService.Domain.ErrorResponses;
using CqrsService.Domain.Orchestrators;
using CqrsService.Shared;

namespace CqrsService.Domain.Services.ExamplePersonModule;

/// <summary>
/// The domain service encapsulates the business rules and logic
/// The domain service calls out to one or more infrastructure orchestrators to build it's domain entities
/// </summary>
public sealed class ExamplePersonService : IExamplePersonService
{
    private readonly IExamplePersonDomainOrchestrator _examplePersonCommandOrchestrator;

    public ExamplePersonService(IExamplePersonDomainOrchestrator examplePersonCommandOrchestrator)
    {
        _examplePersonCommandOrchestrator = examplePersonCommandOrchestrator;
    }

    public async Task<Response<ExamplePerson>> CreatePerson(ExamplePerson model)
    {
        try
        {
            var examplePerson = new ExamplePerson(firstName: model.FirstName,
                lastName: model.LastName,
                age: model.Age);

            //Example of returning a domain entity validation error response
            if (!examplePerson.IsValid())
            {
                return Response<ExamplePerson>.Failure(new DomainValidationErrorResponse(examplePerson, examplePerson.GetValidationErrors()));
            }

            //Example of returning a non domain entity validation error response
            if (examplePerson is null)
            {
                return Response<ExamplePerson>.Failure(new DomainValidationErrorResponse(examplePerson, nameof(examplePerson.FirstName), MessageContext.ErrorExample) );
            }

            var response = await _examplePersonCommandOrchestrator.Create(examplePerson);

            return Response<ExamplePerson>.Success(response);
        }
        catch (Exception ex)
        {
            ExceptionHandlerHelpers.HandleException(exception: ex, className: nameof(ExamplePersonService), methodName: nameof(CreatePerson));
            return Response<ExamplePerson>.Failure(new SystemErrorResponse(ex));
        }
    }

    public async Task<Response<ExamplePerson>> UpdatePerson(ExamplePerson model)
    {
        try
        {
            var examplePerson = new ExamplePerson(firstName: model.FirstName,
                lastName: model.LastName,
                age: model.Age,
                id: model.Id);

            if (!examplePerson.IsValid())
            {
                return Response<ExamplePerson>.Failure(new DomainValidationErrorResponse(examplePerson, examplePerson.GetValidationErrors()));
            }

            var response = await _examplePersonCommandOrchestrator.Update(p => p.Id == examplePerson.Id, examplePerson);

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
            var response = await _examplePersonCommandOrchestrator.Delete<ExamplePerson>(p => p.Id == id);

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
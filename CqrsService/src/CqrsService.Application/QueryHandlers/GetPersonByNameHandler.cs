using CqrsService.Application.Queries;
using CqrsService.Domain.Configuration.Framework;
using CqrsService.Domain.Entities.ExamplePersonModule;
using CqrsService.Domain.ErrorResponses;
using CqrsService.Domain.Orchestrators;
using CqrsService.Shared;
using Mediator;

namespace CqrsService.Application.QueryHandlers;

public sealed class GetPersonByNameHandler : IQueryHandler<GetPersonByNameQuery, Response<ExamplePerson>>
{
    private readonly IExamplePersonDomainOrchestrator _examplePersonDomainOrchestrator;

    public GetPersonByNameHandler(IExamplePersonDomainOrchestrator examplePersonDomainOrchestrator)
    {
        _examplePersonDomainOrchestrator = examplePersonDomainOrchestrator;
    }

    public async ValueTask<Response<ExamplePerson>> Handle(GetPersonByNameQuery query,
        CancellationToken cancellationToken)
    {
        try
        {
            ExamplePerson response = await _examplePersonDomainOrchestrator.GetPersonWithParams<ExamplePerson>(query.FirstName, query.LastName);

            return Response<ExamplePerson>.Success(response);
        }
        catch (Exception ex)
        {
            ExceptionHandlerHelpers.HandleException(ex, nameof(GetPersonByNameHandler), nameof(Handle));
            return Response<ExamplePerson>.Failure(new SystemErrorResponse(ex));
        }
    }
}
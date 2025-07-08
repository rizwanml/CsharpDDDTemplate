using CqrsService.Application.Queries;
using CqrsService.Domain.Configuration.Framework;
using CqrsService.Domain.Entities.ExamplePersonModule;
using CqrsService.Domain.ErrorResponses;
using CqrsService.Domain.Orchestrators;
using CqrsService.Shared;
using Mediator;

namespace CqrsService.Application.QueryHandlers;

public sealed class GetPersonByIdHandler : IQueryHandler<GetPersonByIdQuery, Response<ExamplePerson>>
{
    private readonly IExamplePersonDomainOrchestrator _examplePersonDomainOrchestrator;

    public GetPersonByIdHandler(IExamplePersonDomainOrchestrator examplePersonDomainOrchestrator)
    {
        _examplePersonDomainOrchestrator = examplePersonDomainOrchestrator;
    }

    public async ValueTask<Response<ExamplePerson>> Handle(GetPersonByIdQuery query,
        CancellationToken cancellationToken)
    {
        try
        {
            ExamplePerson response = await _examplePersonDomainOrchestrator.GetById<ExamplePerson>(ep => ep.Id == query.Id);

            return Response<ExamplePerson>.Success(response);
        }
        catch (Exception ex)
        {
            ExceptionHandlerHelpers.HandleException(ex, nameof(GetPersonByIdHandler), nameof(Handle));
            return Response<ExamplePerson>.Failure(new SystemErrorResponse(ex));
        }
    }
}
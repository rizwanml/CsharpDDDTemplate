using CqrsService.Application.Queries;
using CqrsService.Domain.Configuration.Framework;
using CqrsService.Domain.Entities.ExamplePersonModule;
using CqrsService.Domain.ErrorResponses;
using CqrsService.Domain.Orchestrators;
using CqrsService.Shared;
using Mediator;

namespace CqrsService.Application.QueryHandlers;

public sealed class GetAllPersonsHandler : IQueryHandler<GetAllPersonsQuery, Response<IEnumerable<ExamplePerson>>>
{
    private readonly IExamplePersonDomainOrchestrator _examplePersonDomainOrchestrator;

    public GetAllPersonsHandler(IExamplePersonDomainOrchestrator examplePersonDomainOrchestrator)
    {
        _examplePersonDomainOrchestrator = examplePersonDomainOrchestrator;
    }

    public async ValueTask<Response<IEnumerable<ExamplePerson>>> Handle(GetAllPersonsQuery query,
        CancellationToken cancellationToken)
    {
        try
        {
            IEnumerable<ExamplePerson> response =
                await _examplePersonDomainOrchestrator.GetAll<ExamplePerson>(ep => ep.Id != null);

            return Response<IEnumerable<ExamplePerson>>.Success(response);
        }
        catch (Exception ex)
        {
            ExceptionHandlerHelpers.HandleException(ex, nameof(GetAllPersonsHandler), nameof(Handle));
            return Response<IEnumerable<ExamplePerson>>.Failure(new SystemErrorResponse(ex));
        }
    }
}
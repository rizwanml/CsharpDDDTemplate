using CqrsService.Application.Commands;
using CqrsService.Domain.Configuration.Framework;
using CqrsService.Domain.Services.ExamplePersonModule;
using Mediator;

namespace CqrsService.Application.CommandHandlers;

public sealed class CircutBreakerHandler : ICommandHandler<CircuitBreakerCommand, Response<bool>>
{
    private readonly IExamplePersonService _examplePersonService;

    public CircutBreakerHandler(IExamplePersonService examplePersonService)
    {
        _examplePersonService = examplePersonService;
    }

    public async ValueTask<Response<bool>> Handle(CircuitBreakerCommand command,
        CancellationToken cancellationToken)
    {
        return await _examplePersonService.CircuitBreakerResponse();
    }
}
using CqrsService.Application.Commands;
using CqrsService.Domain.Configuration.Framework;
using CqrsService.Domain.Services.ExamplePersonModule;
using Mediator;

namespace CqrsService.Application.CommandHandlers;

public sealed class DeleteExamplePersonHandler : ICommandHandler<DeleteExamplePersonCommand, Response<bool>>
{
    private readonly IExamplePersonService _examplePersonService;

    public DeleteExamplePersonHandler(IExamplePersonService examplePersonService)
    {
        _examplePersonService = examplePersonService;
    }

    public async ValueTask<Response<bool>> Handle(DeleteExamplePersonCommand command,
        CancellationToken cancellationToken)
    {
        return await _examplePersonService.DeletePerson(command.Id);
    }
}
using AutoMapper;
using CqrsService.Application.Commands;
using CqrsService.Domain.Configuration.Framework;
using CqrsService.Domain.Entities.ExamplePersonModule;
using CqrsService.Domain.Services.ExamplePersonModule;
using Mediator;

namespace CqrsService.Application.CommandHandlers;

public sealed class CreateExamplePersonHandler : ICommandHandler<CreateExamplePersonCommand, Response<ExamplePerson>>
{
    private readonly IExamplePersonService _examplePersonService;
    private readonly IMapper _mapper;

    public CreateExamplePersonHandler(IExamplePersonService examplePersonService,
        IMapper mapper)
    {
        _examplePersonService = examplePersonService;
        _mapper = mapper;
    }

    public async ValueTask<Response<ExamplePerson>> Handle(CreateExamplePersonCommand command,
        CancellationToken cancellationToken)
    {
        ExamplePerson? domainModel = _mapper.Map<ExamplePerson>(command);
        return await _examplePersonService.CreatePerson(domainModel);
    }
}
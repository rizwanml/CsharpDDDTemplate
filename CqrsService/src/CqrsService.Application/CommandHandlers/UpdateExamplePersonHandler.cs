using AutoMapper;
using CqrsService.Application.Commands;
using CqrsService.Domain.Configuration.Framework;
using CqrsService.Domain.Entities.ExamplePersonModule;
using CqrsService.Domain.Services.ExamplePersonModule;
using Mediator;

namespace CqrsService.Application.CommandHandlers;

public sealed class UpdateExamplePersonHandler : ICommandHandler<UpdateExamplePersonCommand, Response<ExamplePerson>>
{
    private readonly IExamplePersonService _examplePersonService;
    private readonly IMapper _mapper;

    public UpdateExamplePersonHandler(IExamplePersonService examplePersonService,
        IMapper mapper)
    {
        _examplePersonService = examplePersonService;
        _mapper = mapper;
    }

    public async ValueTask<Response<ExamplePerson>> Handle(UpdateExamplePersonCommand command,
        CancellationToken cancellationToken)
    {
        ExamplePerson? domainModel = _mapper.Map<ExamplePerson>(command);
        return await _examplePersonService.UpdatePerson(domainModel);
    }
}
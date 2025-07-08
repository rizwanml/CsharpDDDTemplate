using CqrsService.Domain.Configuration.Framework;
using CqrsService.Domain.Entities.ExamplePersonModule;
using Mediator;

namespace CqrsService.Application.Commands;

public sealed record UpdateExamplePersonCommand(string Id,
    string FirstName,
    string LastName,
    int Age) : ICommand<Response<ExamplePerson>>;
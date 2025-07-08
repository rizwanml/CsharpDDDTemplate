using CqrsService.Domain.Configuration.Framework;
using Mediator;

namespace CqrsService.Application.Commands;

public sealed record DeleteExamplePersonCommand(string Id) : ICommand<Response<bool>>;
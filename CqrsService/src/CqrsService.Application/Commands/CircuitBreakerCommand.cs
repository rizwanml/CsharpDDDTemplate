using CqrsService.Domain.Configuration.Framework;
using Mediator;

namespace CqrsService.Application.Commands;

public sealed record CircuitBreakerCommand() : ICommand<Response<bool>>;
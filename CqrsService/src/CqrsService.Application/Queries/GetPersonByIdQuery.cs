using CqrsService.Domain.Configuration.Framework;
using CqrsService.Domain.Entities.ExamplePersonModule;
using Mediator;

namespace CqrsService.Application.Queries;

public sealed record GetPersonByIdQuery(string Id)
    : IQuery<Response<ExamplePerson>>;
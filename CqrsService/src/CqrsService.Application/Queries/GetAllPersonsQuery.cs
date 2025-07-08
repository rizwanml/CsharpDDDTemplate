using CqrsService.Domain.Configuration.Framework;
using CqrsService.Domain.Entities.ExamplePersonModule;
using Mediator;

namespace CqrsService.Application.Queries;

public sealed record GetAllPersonsQuery : IQuery<Response<IEnumerable<ExamplePerson>>>;
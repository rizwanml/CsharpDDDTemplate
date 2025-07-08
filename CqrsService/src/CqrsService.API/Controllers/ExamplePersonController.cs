using Asp.Versioning;
using CqrsService.Application.Commands;
using CqrsService.Application.Queries;
using Microsoft.AspNetCore.Mvc;
using Mediator;

namespace CqrsService.API.Controllers;

[ApiVersion("1")]
[ApiVersion("2")]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
public sealed class ExamplePersonController : ControllerBase
{
    private readonly IMediator _mediator;

    public ExamplePersonController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [MapToApiVersion("1")]
    [Obsolete]
    [HttpPost]
    public async Task<IActionResult> CreatePerson(CreateExamplePersonCommand command)
    {
        var response = await _mediator.Send(command);
        return await ApiControllerHelper.HttpResponse(response);
    }

    [MapToApiVersion("2")]
    [HttpPost]
    public async Task<IActionResult> CreatePersonV2(CreateExamplePersonCommand command)
    {
        var response = await _mediator.Send(command);
        return await ApiControllerHelper.HttpResponse(response);
    }

    [MapToApiVersion("1")]
    [HttpGet("[action]")]
    public async Task<IActionResult> GetAllPersons()
    {
        var response = await _mediator.Send(new GetAllPersonsQuery());
        return await ApiControllerHelper.HttpResponse(response);
    }

    [MapToApiVersion("1")]
    [HttpGet("[action]")]
    public async Task<IActionResult> GetPersonById([FromQuery] string id)
    {
        var response = await _mediator.Send(new GetPersonByIdQuery(id));
        return await ApiControllerHelper.HttpResponse(response);
    }

    [MapToApiVersion("1")]
    [HttpGet("[action]")]
    public async Task<IActionResult> GetPersonByFirstNameAndLastName([FromQuery] string firstName, string lastName)
    {
        var response = await _mediator.Send(new GetPersonByNameQuery(firstName, lastName));
        return await ApiControllerHelper.HttpResponse(response);
    }

    [MapToApiVersion("1")]
    [HttpPut]
    public async Task<IActionResult> UpdatePerson(UpdateExamplePersonCommand command)
    {
        var response = await _mediator.Send(command);
        return await ApiControllerHelper.HttpResponse(response);
    }

    [MapToApiVersion("1")]
    [HttpDelete]
    public async Task<IActionResult> DeletePerson(string id)
    {
        var response = await _mediator.Send(new DeleteExamplePersonCommand(id));
        return await ApiControllerHelper.HttpResponse(response);
    }

    [MapToApiVersion("1")]
    [HttpGet("[action]")]
    public async Task<IActionResult> CircuitBreakerResponse()
    {
        var response = await _mediator.Send(new CircuitBreakerCommand());
        return await ApiControllerHelper.HttpResponse(response);
    }
}
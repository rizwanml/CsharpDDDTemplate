using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using SmallService.API.DTOs;
using SmallService.Domain.Entities.ExamplePersonModule;
using SmallService.Domain.Services.ExamplePersonModule;

namespace SmallService.API.Controllers;

[ApiVersion("1")]
[ApiVersion("2")]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
public sealed class ExamplePersonController : ControllerBase
{
    private readonly IExamplePersonService _examplePersonService;

    public ExamplePersonController(IExamplePersonService examplePersonService)
    {
        _examplePersonService = examplePersonService;
    }

    [MapToApiVersion("1")]
    [Obsolete]
    [HttpPost]
    public async Task<IActionResult> CreatePerson(CreateExamplePerson request)
    {
        var examplePerson = new ExamplePerson(request.FirstName, request.LastName, request.Age);

        var response = await _examplePersonService.CreatePerson(examplePerson);
        return await ApiControllerHelper.HttpResponse(response);
    }

    [MapToApiVersion("2")]
    [HttpPost]
    public async Task<IActionResult> CreatePersonV2(CreateExamplePerson request)
    {
        var examplePerson = new ExamplePerson(request.FirstName, request.LastName, request.Age);

        var response = await _examplePersonService.CreatePerson(examplePerson);
        return await ApiControllerHelper.HttpResponse(response);
    }

    [MapToApiVersion("1")]
    [HttpGet("[action]")]
    public async Task<IActionResult> GetAllPersons()
    {
        var response = await _examplePersonService.GetAllPersons();
        return await ApiControllerHelper.HttpResponse(response);
    }

    [MapToApiVersion("1")]
    [HttpGet("[action]")]
    public async Task<IActionResult> GetPersonById([FromQuery] string id)
    {
        var response = await _examplePersonService.GetPersonById(id);
        return await ApiControllerHelper.HttpResponse(response);
    }

    [MapToApiVersion("1")]
    [HttpGet("[action]")]
    public async Task<IActionResult> GetPersonByFirstNameAndLastName([FromQuery] string firstName, string lastName)
    {
        var response = await _examplePersonService.GetPersonWithParams(firstName, lastName);
        return await ApiControllerHelper.HttpResponse(response);
    }

    [MapToApiVersion("1")]
    [HttpPut]
    public async Task<IActionResult> UpdatePerson(UpdateExamplePerson request)
    {
        var examplePerson = new ExamplePerson(request.FirstName, request.LastName, request.Age, request.Id);

        var response = await _examplePersonService.UpdatePerson(examplePerson);
        return await ApiControllerHelper.HttpResponse(response);
    }

    [MapToApiVersion("1")]
    [HttpDelete]
    public async Task<IActionResult> DeletePerson(string id)
    {
        var response = await _examplePersonService.DeletePerson(id);
        return await ApiControllerHelper.HttpResponse(response);
    }

    [MapToApiVersion("1")]
    [HttpGet("[action]")]
    public async Task<IActionResult> CircuitBreakerResponse()
    {
        var response = await _examplePersonService.CircuitBreakerResponse();
        return await ApiControllerHelper.HttpResponse(response);
    }
}
using CqrsService.API.Controllers;
using CqrsService.Application.Commands;
using CqrsService.Application.Queries;
using CqrsService.Domain.Configuration.Framework;
using CqrsService.Domain.Entities.ExamplePersonModule;
using CqrsService.Domain.ErrorResponses;
using Mediator;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace CqrsService.API.Tests.ControllerTests;

public sealed class ExamplePersonControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly ExamplePersonController _examplePersonController;

    public ExamplePersonControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _examplePersonController = new ExamplePersonController(_mediatorMock.Object);
    }

    [Fact]
    public async Task CreatePersonV1_GivenSuccessfulResponse_ShoudReturnOkResult()
    {
        // Arrange
        CreateExamplePersonCommand command = new CreateExamplePersonCommand(FirstName: "FirstName",
            LastName : "LastName",
            Age: 123);

        ExamplePerson examplePerson = new ExamplePerson(command.FirstName,
            command.LastName,
            command.Age);

        var mockResponse = Response<ExamplePerson>.Success(examplePerson);

        _mediatorMock.Setup(r => r.Send(It.IsAny<CreateExamplePersonCommand>(), default))
            .ReturnsAsync(mockResponse);

        // Act
        IActionResult result = await _examplePersonController.CreatePerson(command);

        // Assert
        var objectResult = Assert.IsAssignableFrom<OkObjectResult>(result);
        Assert.NotNull(objectResult);
        Assert.Equal(objectResult.StatusCode, StatusCodes.Status200OK);

        var responseContent = Assert.IsAssignableFrom<ExamplePerson>(objectResult.Value);
        Assert.NotNull(responseContent);
        Assert.Equal(examplePerson, responseContent);
        Assert.False(string.IsNullOrWhiteSpace(responseContent.Id));
        Assert.Equal(responseContent.FirstName, examplePerson.FirstName);
        Assert.Equal(responseContent.LastName, examplePerson.LastName);
        Assert.Equal(responseContent.Age, examplePerson.Age);
    }

    [Fact]
    public async Task CreatePersonV1_GivenDomainValidationErrorResponse_ShoudReturnBadRequestObjectResult()
    {
        // Arrange
        CreateExamplePersonCommand command = new CreateExamplePersonCommand(FirstName: "FirstName",
            LastName : "LastName",
            Age: 123);

        ExamplePerson examplePerson = new ExamplePerson(command.FirstName,
            command.LastName,
            command.Age);

        examplePerson.AddValidationError(nameof(examplePerson.FirstName), "First name is greater than 20.");

        var mockResponse = Response<ExamplePerson>.Failure(new DomainValidationErrorResponse{
            Content = examplePerson,
            ValidationErrors = examplePerson.ValidationErrors
        });

        _mediatorMock.Setup(r => r.Send(It.IsAny<CreateExamplePersonCommand>(), default))
            .ReturnsAsync(mockResponse);

        // Act
        IActionResult result = await _examplePersonController.CreatePerson(command);

        // Assert
        var objectResult = Assert.IsAssignableFrom<BadRequestObjectResult>(result);
        Assert.NotNull(objectResult);
        Assert.Equal(objectResult.StatusCode, StatusCodes.Status400BadRequest);

        var responseContent = Assert.IsAssignableFrom<DomainValidationErrorResponse>(objectResult.Value);
        Assert.NotNull(responseContent);
        Assert.Equal(mockResponse.ErrorResponse, responseContent);

        var content = Assert.IsAssignableFrom<ExamplePerson>(responseContent.Content);
        Assert.NotNull(content);
        Assert.Equal(examplePerson, content);
        Assert.False(string.IsNullOrWhiteSpace(content.Id));
        Assert.Equal(content.FirstName, examplePerson.FirstName);
        Assert.Equal(content.LastName, examplePerson.LastName);
        Assert.Equal(content.Age, examplePerson.Age);

        var validationErrors = Assert.IsAssignableFrom<List<ValidationError>>(responseContent.ValidationErrors);
        Assert.NotNull(validationErrors);
        Assert.NotEmpty(validationErrors);
        Assert.Equal(validationErrors, examplePerson.ValidationErrors);
        Assert.Equal(validationErrors.First().Message, examplePerson.ValidationErrors.First().Message);
        Assert.Equal(validationErrors.First().PropertyName, examplePerson.ValidationErrors.First().PropertyName);
    }

    [Fact]
    public async Task CreatePersonV1_GivenSystemErrorResponse_ShoudReturnObjectResultWith500StatusCode()
    {
        // Arrange
        CreateExamplePersonCommand command = new CreateExamplePersonCommand(FirstName: "FirstName",
            LastName : "LastName",
            Age: 123);

        ExamplePerson examplePerson = new ExamplePerson(command.FirstName,
            command.LastName,
            command.Age);

        var mockResponse = Response<ExamplePerson>.Failure(new SystemErrorResponse{
            ErrorCode = Guid.NewGuid().ToString(),
            ErrorReason = "SystemError",
            ErrorMessage = "A system error occurred while processing the request."
        });

        _mediatorMock.Setup(r => r.Send(It.IsAny<CreateExamplePersonCommand>(), default))
            .ReturnsAsync(mockResponse);

        // Act
        IActionResult result = await _examplePersonController.CreatePerson(command);

        // Assert
        var objectResult = Assert.IsAssignableFrom<ObjectResult>(result);
        Assert.NotNull(objectResult);
        Assert.Equal(objectResult.StatusCode, StatusCodes.Status500InternalServerError);

        var responseContent = Assert.IsAssignableFrom<SystemErrorResponse>(objectResult.Value);
        Assert.NotNull(responseContent);
        Assert.Equal(mockResponse.ErrorResponse, responseContent);
        Assert.Equal(responseContent.ErrorCode, mockResponse.ErrorResponse.ErrorCode);
        Assert.Equal(responseContent.ErrorReason, mockResponse.ErrorResponse.ErrorReason);
        Assert.Equal(responseContent.ErrorMessage, mockResponse.ErrorResponse.ErrorMessage);
    }

    [Fact]
    public async Task GetAllPersonsV1_GivenSuccessfulResponse_ShoudReturnOkResult()
    {
        // Arrange
        var examplePersons = new List<ExamplePerson>();

        for (int i = 0; i < 10; i++)
        {
            var person = new ExamplePerson(firstName: "FirstName" + i,
                lastName: "LastName" + i,
                age: 123);
            examplePersons.Add(person);
        }

        var mockResponse = Response<IEnumerable<ExamplePerson>>.Success(examplePersons);

        _mediatorMock.Setup(m => m.Send(It.IsAny<GetAllPersonsQuery>(), default))
            .ReturnsAsync(mockResponse);

        // Act
        var result = await _examplePersonController.GetAllPersons();

        // Assert
        var objectResult = Assert.IsAssignableFrom<OkObjectResult>(result);
        Assert.NotNull(objectResult);
        Assert.Equal(objectResult.StatusCode, StatusCodes.Status200OK);

        var responseContent = Assert.IsAssignableFrom<IEnumerable<ExamplePerson>>(objectResult.Value);
        Assert.NotNull(responseContent);
        Assert.NotEmpty(responseContent);

        Assert.Equal(responseContent.Count(), examplePersons.Count());

        foreach (var item in responseContent.Select((value, i) => new { i, value }))
        {
            Assert.Equal(item.value.Id, examplePersons[item.i].Id);
            Assert.Equal(item.value.FirstName, examplePersons[item.i].FirstName);
            Assert.Equal(item.value.LastName, examplePersons[item.i].LastName);
            Assert.Equal(item.value.Age, examplePersons[item.i].Age);
        }
    }

    [Fact]
    public async Task GetAllPersonsV1_GivenSystemErrorResponse_ShouldReturnObjectResultWith500StatusCode()
    {
        // Arrange
        var examplePerson = new ExamplePerson(firstName: "FirstName",
            lastName: "LastName",
            age: 123);

        var mockResponse = Response<IEnumerable<ExamplePerson>>.Failure(new SystemErrorResponse{
            ErrorCode = Guid.NewGuid().ToString(),
            ErrorReason = "SystemError",
            ErrorMessage = "A system error occurred while processing the request."
        });

        _mediatorMock.Setup(m => m.Send(It.IsAny<GetAllPersonsQuery>(), default))
            .ReturnsAsync(mockResponse);

        // Act
        var result = await _examplePersonController.GetAllPersons();

        // Assert
        var objectResult = Assert.IsAssignableFrom<ObjectResult>(result);
        Assert.NotNull(objectResult);
        Assert.Equal(objectResult.StatusCode, StatusCodes.Status500InternalServerError);

        var responseContent = Assert.IsAssignableFrom<SystemErrorResponse>(objectResult.Value);
        Assert.NotNull(responseContent);
        Assert.Equal(mockResponse.ErrorResponse, responseContent);
        Assert.Equal(responseContent.ErrorCode, mockResponse.ErrorResponse.ErrorCode);
        Assert.Equal(responseContent.ErrorReason, mockResponse.ErrorResponse.ErrorReason);
        Assert.Equal(responseContent.ErrorMessage, mockResponse.ErrorResponse.ErrorMessage);
    }

    [Fact]
    public async Task GetGetPersonByIdV1_GivenSuccessfulResponse_ShouldReturnOkResult()
    {
        // Arrange
        ExamplePerson examplePerson = new ExamplePerson(firstName: "FirstName", lastName: "LastName", age: 123);

        Response<ExamplePerson> mockResponse = Response<ExamplePerson>.Success(examplePerson);

        _mediatorMock.Setup(m => m.Send(It.IsAny<GetPersonByIdQuery>(), default))
            .ReturnsAsync(mockResponse);

        // Act
        var result = await _examplePersonController.GetPersonById("123");

        // Assert
        var objectResult = Assert.IsAssignableFrom<OkObjectResult>(result);
        Assert.NotNull(objectResult);
        Assert.Equal(objectResult.StatusCode, StatusCodes.Status200OK);

        var responseContent = Assert.IsAssignableFrom<ExamplePerson>(objectResult.Value);
        Assert.NotNull(responseContent);
        Assert.Equal(examplePerson, responseContent);
        Assert.False(string.IsNullOrWhiteSpace(responseContent.Id));
        Assert.Equal(responseContent.FirstName, examplePerson.FirstName);
        Assert.Equal(responseContent.LastName, examplePerson.LastName);
        Assert.Equal(responseContent.Age, examplePerson.Age);
    }

    [Fact]
    public async Task GetGetPersonByIdV1_GivenSystemErrorResponse_ShoudReturnObjectResultWith500StatusCode()
    {
        // Arrange
        ExamplePerson examplePerson = new ExamplePerson(firstName: "FirstName", lastName: "LastName", age: 123);

        Response<ExamplePerson> mockResponse = Response<ExamplePerson>.Failure(new SystemErrorResponse
        {
            ErrorCode = Guid.NewGuid().ToString(),
            ErrorReason = "SystemError",
            ErrorMessage = "A system error occurred while processing the request."
        });

        _mediatorMock.Setup(m => m.Send(It.IsAny<GetPersonByIdQuery>(), default))
            .ReturnsAsync(mockResponse);

        // Act
        var result = await _examplePersonController.GetPersonById("123");

        // Assert
        var objectResult = Assert.IsAssignableFrom<ObjectResult>(result);
        Assert.NotNull(objectResult);
        Assert.Equal(objectResult.StatusCode, StatusCodes.Status500InternalServerError);

        var responseContent = Assert.IsAssignableFrom<SystemErrorResponse>(objectResult.Value);
        Assert.NotNull(responseContent);
        Assert.Equal(mockResponse.ErrorResponse, responseContent);
        Assert.Equal(responseContent.ErrorCode, mockResponse.ErrorResponse.ErrorCode);
        Assert.Equal(responseContent.ErrorReason, mockResponse.ErrorResponse.ErrorReason);
        Assert.Equal(responseContent.ErrorMessage, mockResponse.ErrorResponse.ErrorMessage);
    }
}
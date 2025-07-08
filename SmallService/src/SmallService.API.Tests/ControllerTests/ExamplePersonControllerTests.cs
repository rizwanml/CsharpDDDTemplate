using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SmallService.API.Controllers;
using SmallService.API.DTOs;
using SmallService.Domain.Configuration;
using SmallService.Domain.Configuration.Framework;
using SmallService.Domain.Entities.ExamplePersonModule;
using SmallService.Domain.ErrorResponses;
using SmallService.Domain.Services.ExamplePersonModule;
using Xunit;

namespace SmallService.API.Tests.ControllerTests;

public sealed class ExamplePersonControllerTests
{
    private readonly ExamplePersonController _examplePersonController;
    public static readonly Mock<IExamplePersonService> _examplePersonServiceMock = new();

    public ExamplePersonControllerTests()
    {
        _examplePersonController = new ExamplePersonController(_examplePersonServiceMock.Object);
    }

    [Fact]
    public async Task CreatePersonV1_GivenSuccessfulResponse_ShoudReturnOkResult()
    {
        // Arrange
        var createExamplePerson = new CreateExamplePerson("FirstName", "LastName", 123);

        var examplePerson = new ExamplePerson(firstName: createExamplePerson.FirstName,
            lastName: createExamplePerson.LastName,
            age: createExamplePerson.Age);

        var mockResponse = Response<ExamplePerson>.Success(examplePerson);

        _examplePersonServiceMock.Setup(r => r.CreatePerson(It.IsAny<ExamplePerson>())).ReturnsAsync(mockResponse);

        // Act
        var result = await _examplePersonController.CreatePerson(createExamplePerson);

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
    public async Task CreatePersonV1_GivenDomainValidationErrorResponse_ShouldReturnBadRequestObjectResult()
    {
        // Arrange
        var createExamplePerson = new CreateExamplePerson("FirstName", "LastName", 123);

        var examplePerson = new ExamplePerson(firstName: createExamplePerson.FirstName,
            lastName: createExamplePerson.LastName,
            age: createExamplePerson.Age);

        examplePerson.AddValidationError(nameof(examplePerson.FirstName), "First name is greater than 20.");

        var mockResponse = Response<ExamplePerson>.Failure(new DomainValidationErrorResponse(examplePerson, examplePerson.GetValidationErrors()));

        _examplePersonServiceMock.Setup(r => r.CreatePerson(It.IsAny<ExamplePerson>())).ReturnsAsync(mockResponse);

        // Act
        var result = await _examplePersonController.CreatePerson(createExamplePerson);

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
        Assert.Equal(validationErrors.First().Message, "First name is greater than 20.");
        Assert.Equal(validationErrors.First().PropertyName, nameof(examplePerson.FirstName));
    }

    [Fact]
    public async Task CreatePersonV1_GivenSystemErrorResponse_ShoudReturnObjectResultWith500StatusCode()
    {
        // Arrange
        var dto = new CreateExamplePerson("FirstName", "LastName", 123);

        var mockResponse = Response<ExamplePerson>.Failure(new SystemErrorResponse{
            ErrorCode = Guid.NewGuid().ToString(),
            ErrorReason = "SystemError",
            ErrorMessage = "A system error occurred while processing the request."
        });

        _examplePersonServiceMock.Setup(r => r.CreatePerson(It.IsAny<ExamplePerson>())).ReturnsAsync(mockResponse);

        // Act
        var result = await _examplePersonController.CreatePerson(dto);

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

        _examplePersonServiceMock.Setup(r => r.GetAllPersons()).ReturnsAsync(mockResponse);

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
    public async Task GetAllPersonsV1_GivenSystemErrorResponse_ShoudReturnObjectResultWith500StatusCode()
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

        _examplePersonServiceMock.Setup(r => r.GetAllPersons()).ReturnsAsync(mockResponse);

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
    public async Task GetGetPersonByIdV1_GivenSuccessfulResponse_ShoudReturnOkResult()
    {
        // Arrange
        var examplePerson = new ExamplePerson(firstName: "FirstName",
            lastName: "LastName",
            age: 123);

        var mockResponse = Response<ExamplePerson>.Success(examplePerson);

        _examplePersonServiceMock.Setup(r => r.GetPersonById(It.IsAny<string>())).ReturnsAsync(mockResponse);

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
        var examplePerson = new ExamplePerson(firstName: "FirstName",
            lastName: "LastName",
            age: 123);

        var mockResponse = Response<ExamplePerson>.Failure(new SystemErrorResponse{
            ErrorCode = Guid.NewGuid().ToString(),
            ErrorReason = "SystemError",
            ErrorMessage = "A system error occurred while processing the request."
        });

        _examplePersonServiceMock.Setup(r => r.GetPersonById(It.IsAny<string>())).ReturnsAsync(mockResponse);

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
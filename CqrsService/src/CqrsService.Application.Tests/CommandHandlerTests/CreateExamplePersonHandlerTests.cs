using AutoMapper;
using CqrsService.Application.CommandHandlers;
using CqrsService.Application.Commands;
using CqrsService.Domain.Configuration.Framework;
using CqrsService.Domain.Entities.ExamplePersonModule;
using CqrsService.Domain.Services.ExamplePersonModule;
using Moq;
using Xunit;

namespace CqrsService.Application.Tests.CommandHandlerTests;

public sealed class CreateExamplePersonHandlerTests
{
    private readonly Mock<IExamplePersonService> _examplePersonServiceMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly CreateExamplePersonHandler _handler;

    public CreateExamplePersonHandlerTests()
    {
        _handler = new CreateExamplePersonHandler(_examplePersonServiceMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task CreatePerson()
    {
        // Arrange
        ExamplePerson examplePerson = new ExamplePerson(firstName: "FirstName", lastName: "LastName", age: 123);

        var command = new CreateExamplePersonCommand
        (
            FirstName: "FirstName",
            LastName: "LastName",
            Age: 123
        );

        _examplePersonServiceMock.Setup(r => r.CreatePerson(It.IsAny<ExamplePerson>()))
            .ReturnsAsync(Response<ExamplePerson>.Success(examplePerson));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.IsAssignableFrom<Response<ExamplePerson>>(result);

        ExamplePerson content = result.Content;
        Assert.NotNull(content);
        Assert.IsAssignableFrom<ExamplePerson>(content);

        Assert.False(string.IsNullOrWhiteSpace(content.Id));

        Assert.Equal(content.FirstName, command.FirstName);
        Assert.Equal(content.LastName, command.LastName);
        Assert.Equal(content.Age, command.Age);
    }
}
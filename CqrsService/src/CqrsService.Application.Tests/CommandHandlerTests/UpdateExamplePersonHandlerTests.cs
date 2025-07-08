using AutoMapper;
using CqrsService.Application.CommandHandlers;
using CqrsService.Application.Commands;
using CqrsService.Domain.Configuration.Framework;
using CqrsService.Domain.Entities.ExamplePersonModule;
using CqrsService.Domain.Services.ExamplePersonModule;
using Moq;
using Xunit;

namespace CqrsService.Application.Tests.CommandHandlerTests;

public sealed class UpdateExamplePersonHandlerTests
{
    private readonly Mock<IExamplePersonService> _examplePersonServiceMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly UpdateExamplePersonHandler _handler;

    public UpdateExamplePersonHandlerTests()
    {
        _handler = new UpdateExamplePersonHandler(_examplePersonServiceMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task UpdatePerson()
    {
        // Arrange
        ExamplePerson examplePerson = new ExamplePerson(firstName: "FirstName", lastName: "LastName", age: 123);

        ExamplePerson examplePersonUpdated = new ExamplePerson(firstName: "FirstNameUpdated", lastName: "LastNameUpdated", age: 1234);

        var command = new UpdateExamplePersonCommand(Id: examplePersonUpdated.Id,
            FirstName: examplePersonUpdated.FirstName,
            LastName: examplePersonUpdated.LastName,
            Age: examplePersonUpdated.Age
            );

        _examplePersonServiceMock.Setup(r => r.UpdatePerson(It.IsAny<ExamplePerson>()))
            .ReturnsAsync(Response<ExamplePerson>.Success(examplePersonUpdated));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.IsAssignableFrom<Response<ExamplePerson>>(result);

        ExamplePerson content = result.Content;
        Assert.NotNull(content);
        Assert.IsAssignableFrom<ExamplePerson>(content);

        Assert.False(string.IsNullOrWhiteSpace(content.Id));

        Assert.Equal(content.Id, command.Id);
        Assert.Equal(content.FirstName, command.FirstName);
        Assert.Equal(content.LastName, command.LastName);
        Assert.Equal(content.Age, command.Age);
    }
}
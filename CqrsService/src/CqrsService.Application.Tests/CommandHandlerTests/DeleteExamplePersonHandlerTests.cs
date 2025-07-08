using CqrsService.Application.CommandHandlers;
using CqrsService.Application.Commands;
using CqrsService.Domain.Configuration.Framework;
using CqrsService.Domain.Entities.ExamplePersonModule;
using CqrsService.Domain.Services.ExamplePersonModule;
using Moq;
using Xunit;

namespace CqrsService.Application.Tests.CommandHandlerTests;

public sealed class DeleteExamplePersonHandlerTests
{
    private readonly Mock<IExamplePersonService> _examplePersonServiceMock = new();
    private readonly DeleteExamplePersonHandler _handler;

    public DeleteExamplePersonHandlerTests()
    {
        _handler = new DeleteExamplePersonHandler(_examplePersonServiceMock.Object);
    }

    [Fact]
    public async Task DeletePerson()
    {
        // Arrange
        ExamplePerson examplePerson = new ExamplePerson(firstName: "FirstName", lastName: "LastName", age: 123);

        var command = new DeleteExamplePersonCommand(Id: examplePerson.Id);

        _examplePersonServiceMock.Setup(r => r.DeletePerson(It.IsAny<string>()))
            .ReturnsAsync(Response<bool>.Success(true));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.IsAssignableFrom<Response<bool>>(result);

        bool content = result.Content;
        Assert.NotNull(content);
        Assert.IsAssignableFrom<bool>(content);
    }
}
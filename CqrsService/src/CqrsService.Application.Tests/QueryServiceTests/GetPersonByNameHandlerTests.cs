using CqrsService.Application.QueryHandlers;
using CqrsService.Domain.Configuration.Framework;
using CqrsService.Domain.Entities.ExamplePersonModule;
using CqrsService.Domain.Orchestrators;
using Moq;
using Xunit;

namespace CqrsService.Application.Tests.QueryServiceTests;

public sealed class GetPersonByNameHandlerTests
{
    private readonly Mock<IExamplePersonDomainOrchestrator> _examplePersonDomainOrchestratorMock = new();
    private readonly GetPersonByNameHandler _handler;

    public GetPersonByNameHandlerTests()
    {
        _handler = new GetPersonByNameHandler(_examplePersonDomainOrchestratorMock.Object);
    }

    [Fact]
    public async Task GetPersonWithParams()
    {
        // Arrange
        ExamplePerson examplePerson = new ExamplePerson(firstName: "FirstName", lastName: "LastName", age: 123);

        _examplePersonDomainOrchestratorMock.Setup(r => r.GetPersonWithParams<ExamplePerson>(It.IsAny<object[]>()))
            .ReturnsAsync(examplePerson);

        // Act
        var result = await _handler.Handle(new Queries.GetPersonByNameQuery(examplePerson.FirstName, examplePerson.LastName), CancellationToken.None);

        Assert.NotNull(result);
        Assert.IsAssignableFrom<Response<ExamplePerson>>(result);

        ExamplePerson content = result.Content;
        Assert.NotNull(content);
        Assert.IsAssignableFrom<ExamplePerson>(content);

        Assert.False(string.IsNullOrWhiteSpace(content.Id));

        Assert.Equal(content.Id, examplePerson.Id);
        Assert.Equal(content.FirstName, examplePerson.FirstName);
        Assert.Equal(content.LastName, examplePerson.LastName);
        Assert.Equal(content.Age, examplePerson.Age);
    }
}
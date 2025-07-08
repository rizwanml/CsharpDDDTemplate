using System.Linq.Expressions;
using CqrsService.Application.QueryHandlers;
using CqrsService.Domain.Configuration.Framework;
using CqrsService.Domain.Entities.ExamplePersonModule;
using CqrsService.Domain.Orchestrators;
using Moq;
using Xunit;

namespace CqrsService.Application.Tests.QueryServiceTests;

public sealed class GetAllPersonsHandlerTests
{
    private readonly Mock<IExamplePersonDomainOrchestrator> _examplePersonDomainOrchestratorMock = new();
    private readonly GetAllPersonsHandler _handler;

    public GetAllPersonsHandlerTests()
    {
        _handler = new GetAllPersonsHandler(_examplePersonDomainOrchestratorMock.Object);
    }

    [Fact]
    public async Task GetAllPersons()
    {
        // Arrange
        List<ExamplePerson> examplePersons = new List<ExamplePerson>();

        for (int i = 0; i < 10; i++)
        {
            ExamplePerson examplePerson = new ExamplePerson(firstName: "FirstName" + i,
                lastName: "LastName" + i,
                age: 123);

            examplePersons.Add(examplePerson);
        }

        _examplePersonDomainOrchestratorMock.Setup(r => r.GetAll(It.IsAny<Expression<Func<ExamplePerson, bool>>>()))
            .ReturnsAsync(examplePersons);

        // Act
        var result = await _handler.Handle(new Queries.GetAllPersonsQuery(), CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.IsAssignableFrom<Response<IEnumerable<ExamplePerson>>>(result);

        IEnumerable<ExamplePerson> content = result.Content;
        Assert.NotNull(content);
        Assert.IsAssignableFrom<IEnumerable<ExamplePerson>>(content);

        Assert.Equal(content.Count(), examplePersons.Count());

        foreach (var item in content.Select((value, i) => new { i, value }))
        {
            Assert.Equal(item.value.Id, examplePersons[item.i].Id);
            Assert.Equal(item.value.FirstName, examplePersons[item.i].FirstName);
            Assert.Equal(item.value.LastName, examplePersons[item.i].LastName);
            Assert.Equal(item.value.Age, examplePersons[item.i].Age);
        }
    }
}
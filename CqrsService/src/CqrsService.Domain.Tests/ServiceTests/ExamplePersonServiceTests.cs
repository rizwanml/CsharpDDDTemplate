using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CqrsService.Domain.Configuration.Framework;
using CqrsService.Domain.Entities.ExamplePersonModule;
using CqrsService.Domain.Orchestrators;
using CqrsService.Domain.Services.ExamplePersonModule;
using Moq;
using Xunit;

namespace CqrsService.Domain.Tests.ServiceTests;

public sealed class ExamplePersonServiceTests
{
    private readonly Mock<IExamplePersonDomainOrchestrator> _examplePersonDomainOrchestratorMock = new();
    private readonly IExamplePersonService _examplePersonService;

    public ExamplePersonServiceTests()
    {
        _examplePersonService = new ExamplePersonService(_examplePersonDomainOrchestratorMock.Object);
    }

    [Fact]
    public async Task CreatePerson()
    {
        // Arrange
        ExamplePerson examplePerson = new ExamplePerson(firstName: "FirstName", lastName: "LastName", age: 123);

        _examplePersonDomainOrchestratorMock.Setup(r => r.Create(It.IsAny<ExamplePerson>()))
            .ReturnsAsync(examplePerson);

        // Act
        Response<ExamplePerson> result = await _examplePersonService.CreatePerson(examplePerson);

            // Assert
            Assert.NotNull(result);
            Assert.IsAssignableFrom<Response<ExamplePerson>>(result);

            var content = result.Content;
            Assert.NotNull(content);
            Assert.IsAssignableFrom<ExamplePerson>(content);

            Assert.False(string.IsNullOrWhiteSpace(content.Id));

        Assert.Equal(content.FirstName, examplePerson.FirstName);
        Assert.Equal(content.LastName, examplePerson.LastName);
        Assert.Equal(content.Age, examplePerson.Age);
    }

    [Fact]
    public async Task UpdatePerson()
    {
        // Arrange
        ExamplePerson examplePerson = new ExamplePerson(firstName: "FirstName", lastName: "LastName", age: 123);

        ExamplePerson examplePersonUpdate = new ExamplePerson(id: examplePerson.Id, firstName: "newfirstname", lastName: "newlastname", age: 456);

        _examplePersonDomainOrchestratorMock
            .Setup(r => r.Update(It.IsAny<Expression<Func<ExamplePerson, bool>>>(), It.IsAny<ExamplePerson>()))
            .ReturnsAsync(examplePersonUpdate);

        // Act
        Response<ExamplePerson> result = await _examplePersonService.UpdatePerson(examplePersonUpdate);

            // Assert
            Assert.NotNull(result);
            Assert.IsAssignableFrom<Response<ExamplePerson>>(result);

            var content = result.Content;
            Assert.NotNull(content);
            Assert.IsAssignableFrom<ExamplePerson>(content);

            Assert.False(string.IsNullOrWhiteSpace(content.Id));

        Assert.Equal(content.Id, examplePersonUpdate.Id);
        Assert.Equal(content.FirstName, examplePersonUpdate.FirstName);
        Assert.Equal(content.LastName, examplePersonUpdate.LastName);
        Assert.Equal(content.Age, examplePersonUpdate.Age);
    }

    [Fact]
    public async Task DeletePerson()
    {
        // Arrange
        ExamplePerson examplePerson = new ExamplePerson(firstName: "FirstName", lastName: "LastName", age: 123);

        _examplePersonDomainOrchestratorMock.Setup(r => r.Delete(It.IsAny<Expression<Func<ExamplePerson, bool>>>()))
            .ReturnsAsync(true);

        // Act
        Response<bool> result = await _examplePersonService.DeletePerson(examplePerson.Id);

            // Assert
            Assert.NotNull(result);
            Assert.IsAssignableFrom<Response<bool>>(result);

            var content = result.Content;
            Assert.NotNull(content);
            Assert.IsAssignableFrom<bool>(content);

            Assert.True(content);
        }
}
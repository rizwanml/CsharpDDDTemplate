using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Moq;
using SmallService.Domain.Configuration.Framework;
using SmallService.Domain.Entities.ExamplePersonModule;
using SmallService.Domain.InfrastructureContracts.Processors;
using SmallService.Domain.InfrastructureContracts.Repositories;
using SmallService.Domain.Services.ExamplePersonModule;
using Xunit;

namespace SmallService.Domain.Tests.ServiceTests;

public sealed class ExamplePersonServiceTests
{
    public static readonly Mock<IExamplePersonRepository> _examplePersonRepositoryMock = new();
    public static readonly Mock<IExampleProcessor> _exampleProcessor = new();

    private readonly IExamplePersonService _examplePersonService;

    public ExamplePersonServiceTests()
    {
        _examplePersonService = new ExamplePersonService(_examplePersonRepositoryMock.Object, _exampleProcessor.Object);
    }

    [Fact]
    public async Task CreatePerson()
    {
        // Arrange
        var examplePerson = new ExamplePerson(firstName: "FirstName",
            lastName: "LastName",
            age: 123);

        _examplePersonRepositoryMock.Setup(r => r.Create(It.IsAny<ExamplePerson>())).ReturnsAsync(examplePerson);

        // Act
        var result = await _examplePersonService.CreatePerson(examplePerson);

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
    public async Task GetPersonById()
    {
        // Arrange
        var examplePerson = new ExamplePerson(firstName: "asdfdasfdas",
                                                lastName: "asdfdas",
                                                age: 123);

        _examplePersonRepositoryMock.Setup(r => r.GetById(It.IsAny<Expression<Func<ExamplePerson, bool>>>())).ReturnsAsync(examplePerson);

        // Act
        var result = await _examplePersonService.GetPersonById(examplePerson.Id);

        // Assert
        Assert.NotNull(result);
        Assert.IsAssignableFrom<Response<ExamplePerson>>(result);

        var content = result.Content;
        Assert.NotNull(content);
        Assert.IsAssignableFrom<ExamplePerson>(content);

        Assert.False(string.IsNullOrWhiteSpace(content.Id));

        Assert.Equal(content.Id, examplePerson.Id);
        Assert.Equal(content.FirstName, examplePerson.FirstName);
        Assert.Equal(content.LastName, examplePerson.LastName);
        Assert.Equal(content.Age, examplePerson.Age);
    }

    [Fact]
    public async Task GetAllPersons()
    {
        // Arrange
        var examplePersons = new List<ExamplePerson>();

        for (int i = 0; i < 10; i++)
        {
            var person = new ExamplePerson(firstName: "john" + i,
                                            lastName: "smith" + i,
                                            age: 123);
            examplePersons.Add(person);
        }

        _examplePersonRepositoryMock.Setup(r => r.GetAll(It.IsAny<Expression<Func<ExamplePerson, bool>>>())).ReturnsAsync(examplePersons);

        // Act
        var result = await _examplePersonService.GetAllPersons();

        // Assert
        Assert.NotNull(result);
        Assert.IsAssignableFrom<Response<IEnumerable<ExamplePerson>>>(result);

        var content = result.Content;
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

    [Fact]
    public async Task GetPersonWithParams()
    {
        // Arrange
        var examplePerson = new ExamplePerson(firstName: "asdfdasfdas",
                                                lastName: "asdfdas",
                                                age: 123);

        _examplePersonRepositoryMock.Setup(r => r.GetPersonWithParams<ExamplePerson>(It.IsAny<object[]>())).ReturnsAsync(examplePerson);

        // Act
        var result = await _examplePersonService.GetPersonWithParams(examplePerson.FirstName, examplePerson.LastName);

        Assert.NotNull(result);
        Assert.IsAssignableFrom<Response<ExamplePerson>>(result);

        var content = result.Content;
        Assert.NotNull(content);
        Assert.IsAssignableFrom<ExamplePerson>(content);

        Assert.False(string.IsNullOrWhiteSpace(content.Id));

        Assert.Equal(content.Id, examplePerson.Id);
        Assert.Equal(content.FirstName, examplePerson.FirstName);
        Assert.Equal(content.LastName, examplePerson.LastName);
        Assert.Equal(content.Age, examplePerson.Age);
    }

    [Fact]
    public async Task UpdatePerson()
    {
        // Arrange
        var examplePerson = new ExamplePerson(firstName: "FirstName",
            lastName: "LastName",
            age: 123);

        _examplePersonRepositoryMock.Setup(r => r.Update(It.IsAny<Expression<Func<ExamplePerson, bool>>>(), It.IsAny<ExamplePerson>())).ReturnsAsync(examplePerson);

        // Act
        var result = await _examplePersonService.UpdatePerson(examplePerson);

        // Assert
        Assert.NotNull(result);
        Assert.IsAssignableFrom<Response<ExamplePerson>>(result);

        var content = result.Content;
        Assert.NotNull(content);
        Assert.IsAssignableFrom<ExamplePerson>(content);

        Assert.False(string.IsNullOrWhiteSpace(content.Id));

        Assert.Equal(content.Id, examplePerson.Id);
        Assert.Equal(content.FirstName, examplePerson.FirstName);
        Assert.Equal(content.LastName, examplePerson.LastName);
        Assert.Equal(content.Age, examplePerson.Age);
    }

    [Fact]
    public async Task DeletePerson()
    {
        // Arrange
        var examplePerson = new ExamplePerson(firstName: "FirstName",
            lastName: "LastName",
            age: 123);

        _examplePersonRepositoryMock.Setup(r => r.Delete(It.IsAny<Expression<Func<ExamplePerson, bool>>>())).ReturnsAsync(true);

        // Act
        var result = await _examplePersonService.DeletePerson(examplePerson.Id);

        // Assert
        Assert.NotNull(result);
        Assert.IsAssignableFrom<Response<bool>>(result);

        var content = result.Content;
        Assert.NotNull(content);
        Assert.IsAssignableFrom<bool>(content);

        Assert.True(content);
    }
}
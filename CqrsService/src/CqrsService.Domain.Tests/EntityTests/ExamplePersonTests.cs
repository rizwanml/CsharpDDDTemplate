using CqrsService.Domain.Entities.ExamplePersonModule;
using Xunit;

namespace CqrsService.Domain.Tests.EntityTests;

public sealed class ExamplePersonTests
{
    private static ExamplePerson CreateExamplePerson()
    {
            return new ExamplePerson("FirstName", "LastName", 123, null);
        }

    [Fact]
    public void CreateExamplePerson_GivenFirstNameGreaterThan20_ShouldReturnValidationError()
    {
            // Arrange
            var examplePerson = CreateExamplePerson();
            examplePerson.FirstName = new string('a', 21);

            // Act
            examplePerson.IsValid();

            var validationErrors = examplePerson.GetValidationErrors();

            // Assert
            Assert.True(validationErrors.Exists(a => a.PropertyName == nameof(ExamplePerson.FirstName)));
        }

    [Fact]
    public void CreateExamplePerson_GivenLastNameLessThan5_ShouldReturnValidationError()
    {
            // Arrange
            var examplePerson = CreateExamplePerson();
            examplePerson.LastName = new string('a', 4);

            // Act
            examplePerson.IsValid();

            var validationErrors = examplePerson.GetValidationErrors();

            // Assert
            Assert.True(validationErrors.Exists(a => a.PropertyName == nameof(ExamplePerson.LastName)));
        }

    [Fact]
    public void CreateExamplePerson_GivenAgeLessThan0_ShouldReturnValidationError()
    {
            // Arrange
            var examplePerson = CreateExamplePerson();
            examplePerson.Age = 0;

            // Act
            examplePerson.IsValid();

            var validationErrors = examplePerson.GetValidationErrors();

            // Assert
            Assert.True(validationErrors.Exists(a => a.PropertyName == nameof(ExamplePerson.Age)));
        }
}
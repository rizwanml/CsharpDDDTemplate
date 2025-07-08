namespace CqrsService.Infrastructure.Persistence.Entities;

/// <summary>
/// Example of a persistence entity
/// Entity names (names of tables, collections etc) can be added to the class as a property or data annotation if needed
/// </summary>
public sealed class ExamplePersonDbEntity
{
    public string Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public int Age { get; set; }

    public ExamplePersonDbEntity(string id,
        string firstName,
        string lastName,
        int age)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        Age = age;
    }
}
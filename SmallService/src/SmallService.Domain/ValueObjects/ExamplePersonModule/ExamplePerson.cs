using SmallService.Domain.Configuration.Framework;

namespace SmallService.Domain.ValueObjects.ExamplePersonModule;

public sealed record ExamplePerson : DomainValueObject
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public int Age { get; set; }

    public ExamplePerson(string firstName,
        string lastName,
        int age)
    {
        FirstName = firstName;
        LastName = lastName;
        Age = age;
    }
}
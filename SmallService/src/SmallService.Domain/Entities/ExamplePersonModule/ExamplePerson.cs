using SmallService.Domain.Configuration.Framework;

namespace SmallService.Domain.Entities.ExamplePersonModule;

/// <summary>
/// Example of a domain entity with self validation
/// Validation can be full entity validation or partial
/// </summary>
public sealed class ExamplePerson : DomainEntity
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public int Age { get; set; }

    public ExamplePerson(string firstName,
        string lastName,
        int age,
        string id = null) : base(id)
    {
        FirstName = firstName;
        LastName = lastName;
        Age = age;
    }

    public override bool IsValid()
    {
        if (FirstName.Length > 20)
        {
            AddValidationError(nameof(FirstName), "First name is greater than 20.");
        }

        if (LastName.Length < 5)
        {
            AddValidationError(nameof(LastName), "Last name is less than 5.");
        }

        if (Age <= 0)
        {
            AddValidationError(nameof(Age), "Age is less than 0.");
        }

        return base.IsValid();
    }
}
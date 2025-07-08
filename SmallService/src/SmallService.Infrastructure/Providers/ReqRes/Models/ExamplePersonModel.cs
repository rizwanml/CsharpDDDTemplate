namespace SmallService.Infrastructure.Providers.ReqRes.Models;

/// <summary>
/// Example of a provider model
/// </summary>
public sealed class ExamplePersonModel
{
    public string Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public int Age { get; set; }

    public ExamplePersonModel(string id,
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
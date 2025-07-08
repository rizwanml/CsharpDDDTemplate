using System.Text.Json.Serialization;

namespace SmallService.Infrastructure.Providers.ReqRes.Models;

/// <summary>
/// Example of a provider request model
/// </summary>
public sealed class CreateExamplePersonRequest
{
    [JsonPropertyName("First_Name")]
    public string FirstName { get; set; }

    [JsonPropertyName("Last_Name")]
    public string LastName { get; set; }

    public CreateExamplePersonRequest() { }

    public CreateExamplePersonRequest(string firstName,
        string lastName)
    {
        FirstName = firstName;
        LastName = lastName;
    }
}
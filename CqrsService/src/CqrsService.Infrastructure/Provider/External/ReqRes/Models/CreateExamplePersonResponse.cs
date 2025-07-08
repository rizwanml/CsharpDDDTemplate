using System.Text.Json.Serialization;

namespace CqrsService.Infrastructure.Provider.External.ReqRes.Models;

/// <summary>
/// Example of a provider response model
/// </summary>
public sealed class CreateExamplePersonResponse
{
    [JsonPropertyName("Id")]
    public string Id { get; set; }

    [JsonPropertyName("First_Name")]
    public string FirstName { get; set; }

    [JsonPropertyName("Last_Name")]
    public string LastName { get; set; }

    public CreateExamplePersonResponse(string id,
        string firstName,
        string lastName)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
    }
}
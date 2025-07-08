using System.Text.Json.Serialization;

namespace SmallService.API.DTOs;

public sealed record CreateExamplePerson(
    [property: JsonPropertyName("FirstName")] string FirstName,
    [property: JsonPropertyName("LastName")] string LastName,
    [property: JsonPropertyName("Age")] int Age);
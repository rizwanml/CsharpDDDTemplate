using System.Text.Json.Serialization;

namespace SmallService.API.DTOs;

public record UpdateExamplePerson(
    [property: JsonPropertyName("Id")] string Id,
    [property: JsonPropertyName("FirstName")] string FirstName,
    [property: JsonPropertyName("LastName")] string LastName,
    [property: JsonPropertyName("Age")] int Age);
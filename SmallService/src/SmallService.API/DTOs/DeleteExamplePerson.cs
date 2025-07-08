using System.Text.Json.Serialization;

namespace SmallService.API.DTOs;

public record DeleteExamplePerson( [property: JsonPropertyName("Id")] string Id);
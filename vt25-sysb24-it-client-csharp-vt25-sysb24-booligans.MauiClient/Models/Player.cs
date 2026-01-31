using System.Text.Json.Serialization;

namespace vt25_sysb24_it_client_csharp_vt25_sysb24_booligans.MauiClient.Models;

public record class Player // Matches JSON data received from the API, record class is immutable
{
    [JsonPropertyName("playerNo")] // Maps JSON property to C# property
    public string PlayerNo { get; init; } // init is used to set the value of the property during object initialization, immutable after initialization.

    [JsonPropertyName("position")]
    public string Position { get; init; }

    [JsonPropertyName("playerShirtNo")]
    public string PlayerShirtNo { get; init; }

    [JsonPropertyName("firstName")]
    public string FirstName { get; init; }

    [JsonPropertyName("lastName")]
    public string LastName { get; init; }

    [JsonPropertyName("team")]
    public string Team { get; init; }
}
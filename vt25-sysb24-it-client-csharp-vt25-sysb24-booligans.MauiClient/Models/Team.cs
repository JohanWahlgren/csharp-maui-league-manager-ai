using System.Text.Json.Serialization;
using vt25_sysb24_it_client_csharp_vt25_sysb24_booligans.MauiClient.Models;
using System.Collections.Generic;

namespace vt25_sysb24_it_client_csharp_vt25_sysb24_booligans.MauiClient.Models;

public record class Team // Matches JSON data received from the API, record class is immutable
{
    [JsonPropertyName("teamNo")] // Maps JSON property to C# property
    public string TeamNo { get; init; }

    [JsonPropertyName("teamName")]
    public string TeamName { get; init; }

    [JsonPropertyName("wins")]
    public int Wins { get; init; }

    [JsonPropertyName("losses")]
    public int Losses { get; init; }

    [JsonPropertyName("players")]
    public List<Player> Players { get; init; }
}
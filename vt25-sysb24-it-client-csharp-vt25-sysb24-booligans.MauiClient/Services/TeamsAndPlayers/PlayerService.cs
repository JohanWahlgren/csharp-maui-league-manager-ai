using System.Text.Json;
using vt25_sysb24_it_client_csharp_vt25_sysb24_booligans.MauiClient.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace vt25_sysb24_it_client_csharp_vt25_sysb24_booligans.MauiClient.Services;

public class PlayerService : IPlayerService
{
    private readonly HttpClient _httpClient; // HttpClient is used to send HTTP requests and receive HTTP responses from a web service
    private readonly string _baseUrl = "http://localhost:5119/api/"; // The base URL of the web service

    public PlayerService() // When the PlayerService is created, the HttpClient is initialized with the base URL
    {
        _httpClient = new HttpClient();
        _httpClient.BaseAddress = new Uri(_baseUrl);
    }

    public async Task<IEnumerable<Player>> GetPlayersAsync()
    {
        var responseStream = await _httpClient.GetStreamAsync("players"); // Sends a GET request to the "players" endpoint and returns the response as a stream
        var players = await JsonSerializer.DeserializeAsync<List<Player>>(responseStream) ?? new List<Player>(); // Converts the response stream to a list of Player objects
        return players; // Returns the list to the ViewModel
    }

    public async Task<Player> GetPlayerAsync(string playerNo)
    {
        var responseStream = await _httpClient.GetStreamAsync($"players/{playerNo}"); // Sends a GET request to the "players/{playerNo}" endpoint and returns the response as a stream
        var player = await JsonSerializer.DeserializeAsync<Player>(responseStream); // Converts the response stream to a Player object
        return player;
    }
}
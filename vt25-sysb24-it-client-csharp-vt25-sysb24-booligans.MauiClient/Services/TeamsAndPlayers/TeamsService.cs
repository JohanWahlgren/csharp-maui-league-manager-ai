using vt25_sysb24_it_client_csharp_vt25_sysb24_booligans.MauiClient.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace vt25_sysb24_it_client_csharp_vt25_sysb24_booligans.MauiClient.Services;

public class TeamsService : ITeamService
{
    private readonly HttpClient _httpClient; // HttpClient is used to send HTTP requests and receive HTTP responses from a web service
    private readonly string _baseUrl = "http://localhost:5119/api/"; // The base URL of the web service

    public TeamsService() // When the TeamsService is created, the HttpClient is initialized with the base URL
    {
        _httpClient = new HttpClient();
        _httpClient.BaseAddress = new Uri(_baseUrl);
    }

    public async Task<IEnumerable<Team>> GetTeamsAsync()
    {

        var responseStream = await _httpClient.GetStreamAsync("teams"); // Sends a GET request to the "teams" endpoint and returns the response as a stream
        var teams = await JsonSerializer.DeserializeAsync<List<Team>>(responseStream) ?? new List<Team>(); // Converts the response stream to a list of Team objects
        return teams;
    }

    public async Task<Team> GetTeamAsync(string teamNo)
    {
        var responseStream = await _httpClient.GetStreamAsync($"teams/{teamNo}"); // Sends a GET request to the "players/{teamNo}" endpoint and returns the response as a stream
        var team = await JsonSerializer.DeserializeAsync<Team>(responseStream); // Converts the response stream to a Team object
        return team;
    }
}
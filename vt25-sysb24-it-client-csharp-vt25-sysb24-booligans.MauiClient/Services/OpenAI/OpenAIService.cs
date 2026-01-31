using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace vt25_sysb24_it_client_csharp_vt25_sysb24_booligans.MauiClient.Services
{
    public class OpenAIService : IOpenAIService
    {
        // ==========
        // Fields (readonly = can only be assigned in the constructor or at the point of declaration, and not thereafter)
        // ==========
        private readonly HttpClient _httpClient; // Handles HTTP requests to OpenAI API
        private readonly string _apiKey; // Stores the OpenAI API key

        // ==========
        // Constructor
        // ==========
        public OpenAIService(string apiKey)
        {
            _apiKey = apiKey; // Store the API key in private field
            _httpClient = new HttpClient(); // Create a new HttpClient instance and store in private field
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey); // Accesses authorization headers of the HttpClient, then creates a new value for the header with bearer type of authentication, OpenAI uses Bearer Token Authentication. After this line, every request made by the HttpClient will include the Authorization header with the Bearer token.
        }

        // ==========
        // Public Method: Generate Team Report
        // ==========
        public async Task<string> GenerateTeamReportAsync(string teamInfo, string playerInfo) // Returns a string containing the generated report (or an error message)
        {
            try
            { // Prompt for ChatGPT, uses string interpolation for teamInfo and playerInfo
                string prompt = $@"You are a sports analyst providing concise and informative team analysis.

                                Generate a brief report about this team:

                                Team Information: {teamInfo}

                                Key Players: {playerInfo}

                                Keep your response under 500 words.";

                var requestBody = new // Prepares a JSON object that OpenAI expects
                {
                    model = "gpt-4o-mini",
                    messages = new[]
                    {
                        new { role = "user", content = prompt } // Role and content of the prompt
                    },
                    max_tokens = 700 // Controls length of the response in tokens
                };

                var content = new StringContent( // Converts requestBody to a JSON string and creates a new StringContent object, sets encoding and content type
                    JsonSerializer.Serialize(requestBody),
                    Encoding.UTF8,
                    "application/json");

                var response = await _httpClient.PostAsync("https://api.openai.com/v1/chat/completions", content); // Sends request to OpenAI, awaits response
                response.EnsureSuccessStatusCode(); // Throws an exception if the response is not successful

                var jsonResponse = await response.Content.ReadAsStringAsync(); // Reads the raw JSON from response
                using var doc = JsonDocument.Parse(jsonResponse); // Parses the JSON into a JsonDocument for easier access

                var choices = doc.RootElement.GetProperty("choices"); // Gets the choices array from the JSON
                var firstChoice = choices[0]; // Gets the first choice from the array
                var message = firstChoice.GetProperty("message"); // Gets the message property from the first choice
                var responseText = message.GetProperty("content").GetString(); // Accesses the content field from the message, as a string. The final response from OpenAI.

                return responseText ?? "No response from AI"; // If responseText is null, return a default message
            }
            catch (Exception ex)
            {
                return $"Error generating report: {ex.Message}";
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace NutriDiet.Service.Utilities
{
    public class AIGeneratorService
    {
        private readonly HttpClient _httpClient;
        private readonly string aiApiUrl = Environment.GetEnvironmentVariable("AI_API_URL");
        private readonly string aiApiKey = Environment.GetEnvironmentVariable("AI_API_KEY");

        public AIGeneratorService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> AIResponseText(string text)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, $"{aiApiUrl}?key={aiApiKey}")
            {
                Content = new StringContent(text, Encoding.UTF8, "application/json")
            };

            var response = await _httpClient.SendAsync(request);
            var result = await response.Content.ReadAsStringAsync();
            return result;
        }

        public async Task<string> AIResponseJson(string text, string json)
        {
            var requestBody = new
            {
                contents = new[]
                {
            new
            {
                role = "user",
                parts = new[]
                {
                    new { text = $"input: {text}" },
                    new { text = $"output: {json}" },
                    new { text = $"input: {text}" },
                    new { text = "output: " }
                }
            }
        },
                generationConfig = new
                {
                    temperature = 1,
                    topK = 64,
                    topP = 0.95,
                    maxOutputTokens = 1000,
                    responseMimeType = "text/plain"
                }
            };

            var request = new HttpRequestMessage(HttpMethod.Post, $"{aiApiUrl}?key={aiApiKey}")
            {
                Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json")
            };

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync();
            return result;
        }
    }
}

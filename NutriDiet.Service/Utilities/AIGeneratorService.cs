using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace NutriDiet.Service.Utilities
{
    public class AIGeneratorService
    {
        private readonly HttpClient _httpClient;
        private readonly string aiApiUrl = Environment.GetEnvironmentVariable("AI_API_URL") ?? throw new ArgumentNullException("AI_API_URL", "AI_API_URL environment variable is not set.");
        private readonly string aiApiKey = Environment.GetEnvironmentVariable("AI_API_KEY") ?? throw new ArgumentNullException("AI_API_KEY", "AI_API_KEY environment variable is not set.");

        public AIGeneratorService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> AIResponseText(string input)
        {
            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = $"input: {input}" }
                        }
                    }
                }
            };

            var request = new HttpRequestMessage(HttpMethod.Post, $"{aiApiUrl}?key={aiApiKey}")
            {
                Content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json")
            };

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync();
            return ExtractTextFromAIResponse(result);
        }

        public async Task<string> AIResponseJson(string input, string jsonoutput)
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
                            new { text = $"input: {input}" },
                            new { text = $"output: {jsonoutput}" },
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
                Content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json")
            };

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync();
            return ExtractTextFromAIResponse(result);
        }

        private static string ExtractTextFromAIResponse(string jsonResponse)
        {
            try
            {
                using var doc = JsonDocument.Parse(jsonResponse);
                var root = doc.RootElement;

                if (root.TryGetProperty("candidates", out var candidates) && candidates.GetArrayLength() > 0)
                {
                    var firstCandidate = candidates[0];
                    if (firstCandidate.TryGetProperty("content", out var content) &&
                        content.TryGetProperty("parts", out var parts) &&
                        parts.GetArrayLength() > 0)
                    {
                        return parts[0].GetProperty("text").GetString() ?? "Không có nội dung.";
                    }
                }

                return "Không tìm thấy dữ liệu hợp lệ.";
            }
            catch (Exception ex)
            {
                return $"Lỗi khi xử lý phản hồi từ AI: {ex.Message}";
            }
        }
    }
}
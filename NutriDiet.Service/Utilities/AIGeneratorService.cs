using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace NutriDiet.Service.Utilities
{
    public class AIGeneratorService
    {
        private readonly HttpClient _httpClient;
        private readonly string? aiApiUrl;
        private readonly string? aiApiKey;

        public AIGeneratorService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            aiApiKey = Environment.GetEnvironmentVariable("AI_API_KEY");
            aiApiUrl = Environment.GetEnvironmentVariable("AI_API_URL");
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
                    topK = 40,
                    topP = 0.95,
                    maxOutputTokens = 8192,
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
            var resultText = ExtractTextFromAIResponse(result);
            
            return ExtractJsonFromText(resultText);
        }

        public async Task<string> AIResponseJsonFromImage(string input, IFormFile image, string jsonOutput)
        {
            // ✅ 1. Convert ảnh thành Base64 string
            byte[] imageBytes;
            using (var ms = new MemoryStream())
            {
                await image.CopyToAsync(ms);
                imageBytes = ms.ToArray();
            }
            string base64Image = Convert.ToBase64String(imageBytes);

            var requestBody = new
            {
                contents = new[]
                {
            new
            {
                role = "user",
                parts = new object[]
                {
                    new
                    {
                        inlineData = new
                        {
                            data = base64Image,
                            mimeType = image.ContentType
                        }
                    },
                    new
                    {
                        text = $"Bạn là chuyên gia dinh dưỡng hãy phân tích hình ảnh và trả ra cho tôi output như này: {jsonOutput}\nĐây là ảnh của tôi"
                    }
                }
            }
        },
                generationConfig = new
                {
                    temperature = 1,
                    topK = 40,
                    topP = 0.95,
                    maxOutputTokens = 8192,
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
            var resultText = ExtractTextFromAIResponse(result);

            return ExtractJsonFromText(resultText);
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

        private static string ExtractJsonFromText(string text)
        {
            var match = Regex.Match(text, @"```json\s*(\[.*?\]|\{.*?\})\s*```", RegexOptions.Singleline);
            return match.Success ? match.Groups[1].Value.Trim() : string.Empty;
        }
    }
}
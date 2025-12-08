using System.Text;
using System.Text.Json;

namespace Grandmas_Cooking_MVC.Services
{
    public class OpenAiService
    {
        private readonly string _apiKey;
        private readonly HttpClient _httpClient;

        public OpenAiService(IConfiguration config)
        {
            _apiKey = config["OpenAi:ApiKey"];
            _httpClient = new HttpClient();
        }

        public async Task<string> GetChatResponseAsync(string message)
        {
            var requestBody = new
            {
                model = "gpt-4o-mini",
                messages = new[]
                {
                    new { role = "system", content = "You are a helpful cooking assistant." },
                    new { role = "user", content = message }
                }
            };

            var requestJson = JsonSerializer.Serialize(requestBody);

            var httpRequest = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/chat/completions");

            httpRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _apiKey);

            httpRequest.Content = new StringContent(requestJson, Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(httpRequest);
            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync();

            using var document = JsonDocument.Parse(responseJson);

            var reply = document.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            return reply ?? string.Empty;
        }
    }
}

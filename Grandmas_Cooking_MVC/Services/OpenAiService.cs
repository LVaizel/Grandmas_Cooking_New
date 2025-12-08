// Gaganvir - OpenAiService.cs created for integrating OpenAI's chat functionality.

using System.Text;
using System.Text.Json;

//https://www.youtube.com/watch?v=55fLTVYXpdg
//https://zenkins.com/career-insights/integrating-chatgpt-in-net-core/
//https://dtoyoda10.medium.com/building-a-c-chatbot-with-chatgpt-8f2056f90b10
//https://github.com/OkGoDoIt/OpenAI-API-dotnet?tab=readme-ov-file#chat-api
// used the link above as a reference

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

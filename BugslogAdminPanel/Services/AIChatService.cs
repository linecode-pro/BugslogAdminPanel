using BugslogAdminPanel.Data;
using System.Net.Http;
using System.Text.Json;

namespace BugslogAdminPanel.Services
{
    public interface IAIChatService
    {
        public Task<ExecutionResult> MakeRequestToAIAsync(string message);
    }

    public class AIChatService(IHttpClientFactory _httpClientFactory, IConfiguration _configuration) : IAIChatService
    {
        public async Task<ExecutionResult> MakeRequestToAIAsync(string message)
        {
            try
            {
                if (string.IsNullOrEmpty(message))
                    throw new ArgumentNullException(nameof(message));
                
                // Использовать using!
                using var client = _httpClientFactory.CreateClient();

                client.DefaultRequestHeaders.Add("Accept", "application/json");
                client.DefaultRequestHeaders.Add("User-Agent", "BugslogAdminPanel");

                string apiUrl = _configuration["AIChatApiUrl"] ?? throw new Exception("Ошибка! В файле 'appsettings.json' не задан параметр 'AIChatApiUrl'");
                client.BaseAddress = new Uri(apiUrl);

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true // Делает десериализацию нечувствительной к регистру
                };

                var response = await client!.PostAsJsonAsync("tags", message.Trim());

                if (!response.IsSuccessStatusCode)
                {
                    string errorText = await response.Content.ReadAsStringAsync();
                    throw new Exception(errorText);
                }

                string apiResponse = await response.Content.ReadAsStringAsync();

                apiResponse = apiResponse.Replace("\"", "");

                return new ExecutionResult() { CompletedSuccessfully = true, Data = apiResponse };
            }
            catch (Exception ex)
            {
                return new ExecutionResult() { ErrorText = ex.Message };
            }
        }
    }
}

using BugslogAdminPanel.Data;
using System.Linq.Dynamic.Core.Tokenizer;
using System.Text;
using System.Text.Json;

namespace BugslogAdminPanel.Services
{
    public interface IPromptService
    {
        public Task<ExecutionResult> GetAllPromptsAsync();

        public Task<ExecutionResult> AddPromptAsync(SimplePrompt prompt);

        public Task<ExecutionResult> UpdatePromptAsync(SimplePrompt prompt);

        public Task<ExecutionResult> DeletePromptAsync(SimplePrompt prompt);

        public Task<ExecutionResult> SetDefaultPromptsAsync();

        public Task<ExecutionResult> GetDefaultPromptAsync();
    }

    public class PromptService(IHttpClientFactory _httpClientFactory, IConfiguration _configuration) : IPromptService
    {
        public async Task<ExecutionResult> GetAllPromptsAsync()
        {
            try
            {
                // Использовать using!
                using var client = _httpClientFactory.CreateClient();

                client.DefaultRequestHeaders.Add("Accept", "application/json");
                client.DefaultRequestHeaders.Add("User-Agent", "BugslogAdminPanel");

                string apiUrl = _configuration["ApiNoSqlBaseUrl"] ?? throw new Exception("Ошибка! В файле 'appsettings.json' не задан параметр 'ApiNoSqlBaseUrl'");
                client.BaseAddress = new Uri(apiUrl);

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true // Делает десериализацию нечувствительной к регистру
                };

                var response = await client!.GetAsync("prompts");

                if (!response.IsSuccessStatusCode)
                {
                    string errorText = await response.Content.ReadAsStringAsync();
                    throw new Exception(errorText);
                }

                string apiResponse = await response.Content.ReadAsStringAsync();
                List<SimplePrompt> prompts = JsonSerializer.Deserialize<List<SimplePrompt>>(apiResponse, options) ?? new();

                return new ExecutionResult() { CompletedSuccessfully = true, Data = prompts };
            }
            catch (Exception ex)
            {
                return new ExecutionResult() { ErrorText = ex.Message };
            }
        }

        public async Task<ExecutionResult> AddPromptAsync(SimplePrompt prompt)
        {
            try
            {
                // Использовать using!
                using var client = _httpClientFactory.CreateClient();

                client.DefaultRequestHeaders.Add("Accept", "application/json");
                client.DefaultRequestHeaders.Add("User-Agent", "BugslogAdminPanel");

                string apiUrl = _configuration["ApiNoSqlBaseUrl"] ?? throw new Exception("Ошибка! В файле 'appsettings.json' не задан параметр 'ApiNoSqlBaseUrl'");
                client.BaseAddress = new Uri(apiUrl);

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true // Делает десериализацию нечувствительной к регистру
                };

                var response = await client!.PostAsJsonAsync("prompts", prompt);

                if (!response.IsSuccessStatusCode)
                {
                    string errorText = await response.Content.ReadAsStringAsync();
                    throw new Exception(errorText);
                }

                string apiResponse = await response.Content.ReadAsStringAsync();
                SimplePrompt newPrompt = JsonSerializer.Deserialize<SimplePrompt>(apiResponse, options) ?? throw new Exception("Ошибка десериализации созданного промта.");

                return new ExecutionResult() { CompletedSuccessfully = true, Data = newPrompt };
            }
            catch (Exception ex)
            {
                return new ExecutionResult() { ErrorText = ex.Message };
            }
        }

        public async Task<ExecutionResult> UpdatePromptAsync(SimplePrompt prompt)
        {
            try
            {
                // Использовать using!
                using var client = _httpClientFactory.CreateClient();

                client.DefaultRequestHeaders.Add("Accept", "application/json");
                client.DefaultRequestHeaders.Add("User-Agent", "BugslogAdminPanel");

                string apiUrl = _configuration["ApiNoSqlBaseUrl"] ?? throw new Exception("Ошибка! В файле 'appsettings.json' не задан параметр 'ApiNoSqlBaseUrl'");
                client.BaseAddress = new Uri(apiUrl);

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true // Делает десериализацию нечувствительной к регистру
                };

                var response = await client!.PutAsJsonAsync("prompts", prompt);

                if (!response.IsSuccessStatusCode)
                {
                    string errorText = await response.Content.ReadAsStringAsync();
                    throw new Exception(errorText);
                }

                string apiResponse = await response.Content.ReadAsStringAsync();
                SimplePrompt newPrompt = JsonSerializer.Deserialize<SimplePrompt>(apiResponse, options) ?? throw new Exception("Ошибка десериализации промта.");

                return new ExecutionResult() { CompletedSuccessfully = true, Data = newPrompt };
            }
            catch (Exception ex)
            {
                return new ExecutionResult() { ErrorText = ex.Message };
            }
        }

        public async Task<ExecutionResult> DeletePromptAsync(SimplePrompt prompt)
        {
            try
            {
                // Использовать using!
                using var client = _httpClientFactory.CreateClient();

                client.DefaultRequestHeaders.Add("Accept", "application/json");
                client.DefaultRequestHeaders.Add("User-Agent", "BugslogAdminPanel");

                string apiUrl = _configuration["ApiNoSqlBaseUrl"] ?? throw new Exception("Ошибка! В файле 'appsettings.json' не задан параметр 'ApiNoSqlBaseUrl'");
                client.BaseAddress = new Uri(apiUrl);

                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Delete,
                    RequestUri = new Uri(apiUrl + "/prompts"),
                    Content = new StringContent(JsonSerializer.Serialize(prompt), Encoding.UTF8, "application/json")
                };
                var response = await client.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    string errorText = await response.Content.ReadAsStringAsync();
                    throw new Exception(errorText);
                }

                return new ExecutionResult() { CompletedSuccessfully = true };
            }
            catch (Exception ex)
            {
                return new ExecutionResult() { ErrorText = ex.Message };
            }
        }

        public async Task<ExecutionResult> SetDefaultPromptsAsync()
        {
            try
            {
                // Использовать using!
                using var client = _httpClientFactory.CreateClient();

                client.DefaultRequestHeaders.Add("Accept", "application/json");
                client.DefaultRequestHeaders.Add("User-Agent", "BugslogAdminPanel");

                string apiUrl = _configuration["ApiNoSqlBaseUrl"] ?? throw new Exception("Ошибка! В файле 'appsettings.json' не задан параметр 'ApiNoSqlBaseUrl'");
                client.BaseAddress = new Uri(apiUrl);

                var response = await client!.PostAsync("setdefaultprompts", null);

                if (!response.IsSuccessStatusCode)
                {
                    string errorText = await response.Content.ReadAsStringAsync();
                    throw new Exception(errorText);
                }

                return new ExecutionResult() { CompletedSuccessfully = true };
            }
            catch (Exception ex)
            {
                return new ExecutionResult() { ErrorText = ex.Message };
            }
        }

        public async Task<ExecutionResult> GetDefaultPromptAsync()
        {
            try
            {
                // Использовать using!
                using var client = _httpClientFactory.CreateClient();

                client.DefaultRequestHeaders.Add("Accept", "application/json");
                client.DefaultRequestHeaders.Add("User-Agent", "BugslogAdminPanel");

                string apiUrl = _configuration["ApiNoSqlBaseUrl"] ?? throw new Exception("Ошибка! В файле 'appsettings.json' не задан параметр 'ApiNoSqlBaseUrl'");
                client.BaseAddress = new Uri(apiUrl);

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true // Делает десериализацию нечувствительной к регистру
                };

                var response = await client!.GetAsync("getdefaultprompt");

                if (!response.IsSuccessStatusCode)
                {
                    string errorText = await response.Content.ReadAsStringAsync();
                    throw new Exception(errorText);
                }

                string apiResponse = await response.Content.ReadAsStringAsync();
                SimplePrompt prompt = JsonSerializer.Deserialize<SimplePrompt>(apiResponse, options) ?? throw new Exception("Промт по умолчанию не получен!");

                return new ExecutionResult() { CompletedSuccessfully = true, Data = prompt };
            }
            catch (Exception ex)
            {
                return new ExecutionResult() { ErrorText = ex.Message };
            }
        }
    }
}

using BugslogAdminPanel.Data;
using System.Linq.Dynamic.Core.Tokenizer;
using System.Text;
using System.Text.Json;

namespace BugslogAdminPanel.Services
{
    public interface ITagService
    {
        public Task<ExecutionResult> GetAllTagsAsync();

        public Task<ExecutionResult> AddTagAsync(SimpleTag tag);

        public Task<ExecutionResult> UpdateTagAsync(SimpleTag tag);

        public Task<ExecutionResult> DeleteTagAsync(SimpleTag tag);

        public Task<ExecutionResult> SetDefaultTagsAsync();
    }

    public class TagService(IHttpClientFactory _httpClientFactory, IConfiguration _configuration) : ITagService
    {
        public async Task<ExecutionResult> GetAllTagsAsync()
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

                var response = await client!.GetAsync("tags");

                if (!response.IsSuccessStatusCode)
                {
                    string errorText = await response.Content.ReadAsStringAsync();
                    throw new Exception(errorText);
                }

                string apiResponse = await response.Content.ReadAsStringAsync();
                List<SimpleTag> tags = JsonSerializer.Deserialize<List<SimpleTag>>(apiResponse, options) ?? new ();

                return new ExecutionResult() { CompletedSuccessfully = true, Data = tags };
            }
            catch (Exception ex)
            {
                return new ExecutionResult() { ErrorText = ex.Message };
            }
        }

        public async Task<ExecutionResult> AddTagAsync(SimpleTag tag)
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

                var response = await client!.PostAsJsonAsync("tags", tag);

                if (!response.IsSuccessStatusCode)
                {
                    string errorText = await response.Content.ReadAsStringAsync();
                    throw new Exception(errorText);
                }

                string apiResponse = await response.Content.ReadAsStringAsync();
                SimpleTag newTag = JsonSerializer.Deserialize<SimpleTag>(apiResponse, options) ?? throw new Exception("Ошибка десериализации созданного тега.");

                return new ExecutionResult() { CompletedSuccessfully = true, Data = newTag };
            }
            catch (Exception ex)
            {
                return new ExecutionResult() { ErrorText = ex.Message };
            }
        }

        public async Task<ExecutionResult> UpdateTagAsync(SimpleTag tag)
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

                var response = await client!.PutAsJsonAsync("tags", tag);

                if (!response.IsSuccessStatusCode)
                {
                    string errorText = await response.Content.ReadAsStringAsync();
                    throw new Exception(errorText);
                }

                string apiResponse = await response.Content.ReadAsStringAsync();
                SimpleTag newTag = JsonSerializer.Deserialize<SimpleTag>(apiResponse, options) ?? throw new Exception("Ошибка десериализации тега.");

                return new ExecutionResult() { CompletedSuccessfully = true, Data = newTag };
            }
            catch (Exception ex)
            {
                return new ExecutionResult() { ErrorText = ex.Message };
            }
        }

        public async Task<ExecutionResult> DeleteTagAsync(SimpleTag tag)
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
                    RequestUri = new Uri(apiUrl + "/tags"),
                    Content = new StringContent(JsonSerializer.Serialize(tag), Encoding.UTF8, "application/json")
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

        public async Task<ExecutionResult> SetDefaultTagsAsync()
        {
            try
            {
                // Использовать using!
                using var client = _httpClientFactory.CreateClient();

                client.DefaultRequestHeaders.Add("Accept", "application/json");
                client.DefaultRequestHeaders.Add("User-Agent", "BugslogAdminPanel");

                string apiUrl = _configuration["ApiNoSqlBaseUrl"] ?? throw new Exception("Ошибка! В файле 'appsettings.json' не задан параметр 'ApiNoSqlBaseUrl'");
                client.BaseAddress = new Uri(apiUrl);

                var response = await client!.PostAsync("setdefaulttags", null);

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
    }
}

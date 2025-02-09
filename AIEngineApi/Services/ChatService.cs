using AIEngineApi.Data;
using System.Net.Http.Headers;
using System.Net;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace AIEngineApi.Services
{
    public interface IChatService
    {
        public Task<ExecutionResult> GetTagByMessageAsync(string message);
    }

    public class ChatService(IHttpClientFactory _httpClientFactory, IConfiguration _configuration) : IChatService
    {
        public async Task<ExecutionResult> GetTagByMessageAsync(string message)
        {
            try
            {
                // 1. Получить промт и теги из базы данных
                
                // 1.1
                ExecutionResult result = await GetDefaultPromptAsync();

                if (!result.CompletedSuccessfully)
                {
                    return result;
                }

                var prompt = result?.Data as SimplePrompt ?? new();
                string promptText = prompt.Message;

                // 1.2
                result = await GetAllTagsAsync();

                if (!result.CompletedSuccessfully)
                {
                    return result;
                }
                string tagsList = string.Empty;

                var tags = result?.Data as List<SimpleTag> ?? new();

                foreach (var item in tags)
                {
                    tagsList = tagsList + ((tagsList == string.Empty) ? "" : ",") + $"'{item.Name}'";
                }



                // 2. Составить полный итоговый текст промта
                string requestString = promptText + "\n Список тегов: " + tagsList + "\n Обращение: " + message;



                // 3. Сделать запрос к нейросети
                string answer = await MadeRequest(requestString);

                return new ExecutionResult() { CompletedSuccessfully = true, Data = answer.Trim() };
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
                List<SimpleTag> tags = JsonSerializer.Deserialize<List<SimpleTag>>(apiResponse, options) ?? new();

                return new ExecutionResult() { CompletedSuccessfully = true, Data = tags };
            }
            catch (Exception ex)
            {
                return new ExecutionResult() { ErrorText = ex.Message };
            }
        }


        #region GigaChat   
        async Task<string> MadeRequest(string requestString)
        {
            // Использовать using!
            using var httpClient = _httpClientFactory.CreateClient();

            // Получить токен доступа и сделать запрос к нейросети
            var token = await GetTokenAsync(httpClient);
            Root generateResult = await GetGenerateResultAsync(token, httpClient, requestString);

            string answer = generateResult.choices[0].message.content;

            answer = answer.Replace("\"", "");

            return answer;
        }

        /// <summary>
        /// Получить токен доступа
        /// </summary>
        /// <param name="httpClient"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<string> GetTokenAsync(HttpClient httpClient)
        {
            string Credentials = _configuration["GigaChat:Credentials"] ?? throw new Exception("Ошибка! В файле 'appsettings.json' не задан параметр 'Credentials'");
            string url_token = _configuration["GigaChat:ServerURL_Token"] ?? throw new Exception("Ошибка! В файле 'appsettings.json' не задан параметр 'ServerURL_Token'");
            string rqUID = _configuration["GigaChat:rqUID"] ?? throw new Exception("Ошибка! В файле 'appsettings.json' не задан параметр 'rqUID'");

            var request = new HttpRequestMessage(HttpMethod.Post, url_token);
            request.Headers.Add("Accept", "application/json");
            request.Headers.Add("Authorization", $"Basic {Credentials}");
            request.Headers.Add("RqUID", rqUID);
            var collection = new List<KeyValuePair<string, string>>();
            collection.Add(new("scope", "GIGACHAT_API_PERS"));
            var content = new FormUrlEncodedContent(collection);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
            request.Content = content;

            try
            {
                var response = await httpClient.SendAsync(request);
                if (!response.IsSuccessStatusCode)
                    throw new Exception($"Ошибка получения токена: {(int)response.StatusCode} {response.ReasonPhrase}");

                var responseString = await response.Content.ReadAsStringAsync();
                var tokenValue = JsonDocument.Parse(responseString).RootElement;

                JsonNode jsonNode = JsonNode.Parse(responseString)!;

                string token = (string)jsonNode["access_token"]!;

                return token;
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Ошибка при получении токена: {ex.Message} : {ex.InnerException?.Message ?? string.Empty}");
            }
        }

        /// <summary>
        /// Сделать запрос и получить результат
        /// </summary>
        /// <param name="token"></param>
        /// <param name="httpClient"></param>
        /// <param name="requestString"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<dynamic> GetGenerateResultAsync(string token, HttpClient httpClient, string requestString)
        {
            string url = _configuration["GigaChat:ServerURL_Request"] ?? throw new Exception("Ошибка! В файле 'appsettings.json' не задан параметр 'ServerURL_Request'");

            var jsonContent = JsonContent.Create(new
            {
                model = "GigaChat",
                messages = new List<MessageTo>()
            {
                new MessageTo { Role = "system", Content = "" },
                new MessageTo
                {
                    Role = "user",
                    Content = requestString
                }
            },
                profanity_check = true
            });

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            try
            {
                var response = await httpClient.PostAsync(url, jsonContent);

                if (!response.IsSuccessStatusCode)
                    throw new Exception($"Ошибка выполнения запроса: {(int)response.StatusCode} {response.ReasonPhrase}");

                var result = await response.Content.ReadFromJsonAsync<dynamic>();

                string jsonResponse = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                };

                Root resultClass = JsonSerializer.Deserialize<Root>(jsonResponse, options)!;

                return resultClass;
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Ошибка при выполнении запроса: {ex.Message}");
            }
        }

        #endregion
    }


    #region SupportClasses

#pragma warning disable CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Рассмотрите возможность добавления модификатора "required" или объявления значения, допускающего значение NULL.

    public class MessageTo
    {
        public string Role { get; set; }
        public string Content { get; set; }
        public long? CreatedAt { get; set; }
    }
    public class Choice
    {
        public Message message { get; set; }
        public int index { get; set; }
        public string finish_reason { get; set; }
    }

    public class Message
    {
        public string content { get; set; }
        public string role { get; set; }
    }

    public class Root
    {
        public List<Choice> choices { get; set; }
        public int created { get; set; }
        public string model { get; set; }
        public string @object { get; set; }
        public Usage usage { get; set; }
    }

    public class Usage
    {
        public int prompt_tokens { get; set; }
        public int completion_tokens { get; set; }
        public int total_tokens { get; set; }
    }

#pragma warning restore CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Рассмотрите возможность добавления модификатора "required" или объявления значения, допускающего значение NULL.

    #endregion
}

using NoSqlBaseApi.Data;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;
using System.Text.RegularExpressions;

namespace NoSqlBaseApi.Repositories
{
    public interface IPromptRepository
    {
        public Task<ExecutionResult> GetAllPromptsAsync();

        public Task<ExecutionResult> CreateNewPromptAsync(SimplePrompt prompt);

        public Task<ExecutionResult> UpdatePromptAsync(SimplePrompt prompt);

        public Task<ExecutionResult> DeletePromptAsync(SimplePrompt prompt);

        public Task<ExecutionResult> SetDefaultPromptsAsync();

        public Task<ExecutionResult> GetDefaultPromptAsync();
    }

    public class PromptRepository() : IPromptRepository
    {
        public async Task<ExecutionResult> GetAllPromptsAsync()
        {
            try
            {
                List<SimplePrompt> prompts = new();

                using (IAsyncDocumentSession asyncSession = DocumentStoreHolder.Store.OpenAsyncSession())
                {
                    // Получить все промты
                    prompts = await asyncSession.Query<SimplePrompt>().OrderBy(x => x.Message).ToListAsync();
                }

                return new() { CompletedSuccessfully = true, Data = prompts };
            }
            catch (Exception ex)
            {
                return new ExecutionResult() { ErrorText = "Ошибка при работе с базой данных: " + ex.Message };
            }
        }

        public async Task<ExecutionResult> CreateNewPromptAsync(SimplePrompt prompt)
        {
            try
            {
                using (IAsyncDocumentSession asyncSession = DocumentStoreHolder.Store.OpenAsyncSession())
                {
                    // Добавить новый промт в базу данных
                    if (!prompt.Default)
                    {
                        // Просто добавить новый промт в базу
                        await asyncSession.StoreAsync(prompt);
                    }
                    else
                    {
                        // 1. Получить все промты, где 'Default' = true
                        // и убрать там этот признак
                        var prompts = await asyncSession.Query<SimplePrompt>().Where(x => x.Default == true).ToListAsync();
                        foreach (var item in prompts)
                        {
                            item.Default = false;
                        }

                        // 2. Просто добавить новый промт в базу
                        await asyncSession.StoreAsync(prompt);
                    }

                    await asyncSession.SaveChangesAsync(); // Сохраняем изменения
                }

                return new() { CompletedSuccessfully = true, Data = prompt };
            }
            catch (Exception ex)
            {
                return new ExecutionResult() { ErrorText = "Ошибка при работе с базой данных: " + ex.Message };
            }
        }

        public async Task<ExecutionResult> UpdatePromptAsync(SimplePrompt prompt)
        {
            try
            {
                using (IAsyncDocumentSession asyncSession = DocumentStoreHolder.Store.OpenAsyncSession())
                {
                    // Записать промт в базу данных
                    if (!prompt.Default)
                    {
                        // Просто добавить новый промт в базу
                        await asyncSession.StoreAsync(prompt);
                    }
                    else
                    {
                        // 1. Получить все промты, где 'Default' = true
                        // и убрать там этот признак
                        var prompts = await asyncSession.Query<SimplePrompt>().Where(x => x.Default == true || x.Id == prompt.Id).ToListAsync();
                        foreach (var item in prompts)
                        {
                            if (item.Id != prompt.Id)
                            {
                                item.Default = false;
                            }
                            else
                            {
                                item.Message = prompt.Message;
                                item.Default = prompt.Default;
                            }
                        }    
                    }

                    await asyncSession.SaveChangesAsync(); // Сохранить изменения
                }

                return new() { CompletedSuccessfully = true, Data = prompt };
            }
            catch (Exception ex)
            {
                return new ExecutionResult() { ErrorText = "Ошибка при работе с базой данных: " + ex.Message };
            }
        }

        public async Task<ExecutionResult> DeletePromptAsync(SimplePrompt prompt)
        {
            try
            {
                using (IAsyncDocumentSession asyncSession = DocumentStoreHolder.Store.OpenAsyncSession())
                {
                    // Удалить промт из базы данных
                    asyncSession.Delete(prompt.Id);

                    await asyncSession.SaveChangesAsync(); // Сохраняем изменения
                }

                return new() { CompletedSuccessfully = true };
            }
            catch (Exception ex)
            {
                return new ExecutionResult() { ErrorText = "Ошибка при работе с базой данных: " + ex.Message };
            }
        }

        public async Task<ExecutionResult> SetDefaultPromptsAsync()
        {
            try
            {
                using (IAsyncDocumentSession asyncSession = DocumentStoreHolder.Store.OpenAsyncSession())
                {
                    // 1. Получить все промты
                    var prompts = await asyncSession.Query<SimplePrompt>().ToListAsync();

                    // 2. Удалить промты
                    foreach (var document in prompts)
                    {
                        asyncSession.Delete(document);
                    }

                    // 3. Добавить новый промт
                    string promptText = @"Ты - робот, рассматривающий обращения пользователей в службу поддержки и определяющий тему обращения.
                                    Каждому обращению надо присвоить тег (тему) из списка тегов. В ответ выводи только тег без двойных кавычек.
                                    ";

                    promptText = promptText.Replace("\n", "").Replace("\r", "").Replace("\t", "");

                    // Убрать лишние пробелы между словами
                    promptText = Regex.Replace(promptText, @"\s+", " ");

                    // Обрезать начальные и конечные пробелы
                    promptText = promptText.Trim();

                    SimplePrompt prompt = new() {
                        Message = promptText,
                        Default = true                        
                    };
                    await asyncSession.StoreAsync(prompt);
                    
                    await asyncSession.SaveChangesAsync(); // Сохранить изменения
                }

                return new() { CompletedSuccessfully = true };
            }
            catch (Exception ex)
            {
                return new ExecutionResult() { ErrorText = "Ошибка при работе с базой данных: " + ex.Message };
            }
        }

        public async Task<ExecutionResult> GetDefaultPromptAsync()
        {
            try
            {
                SimplePrompt? prompt;

                using (IAsyncDocumentSession asyncSession = DocumentStoreHolder.Store.OpenAsyncSession())
                {
                    // Получить все промты, где 'Default' = true
                    // и взять первый
                    var prompts = await asyncSession.Query<SimplePrompt>().Where(x => x.Default == true).ToListAsync();
                    if (prompts is null || prompts.Count == 0)
                    {
                        throw new Exception("Ошибка! Не задан промт по умолчанию.");   
                    }
                    else
                    {
                        prompt = prompts.FirstOrDefault();
                    }                    
                }

                return new() { CompletedSuccessfully = true, Data = prompt };
            }
            catch (Exception ex)
            {
                return new ExecutionResult() { ErrorText = "Ошибка при работе с базой данных: " + ex.Message };
            }
        }
    }
}

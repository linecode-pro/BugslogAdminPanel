using NoSqlBaseApi.Data;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;

namespace NoSqlBaseApi.Repositories
{
    public interface ITagRepository
    {
        public Task<ExecutionResult> GetAllTagsAsync();

        public Task<ExecutionResult> CreateNewTagAsync(SimpleTag tag);

        public Task<ExecutionResult> UpdateTagAsync(SimpleTag tag);

        public Task<ExecutionResult> DeleteTagAsync(SimpleTag tag);

        public Task<ExecutionResult> SetDefaultTagsAsync();
    }

    public class TagRepository() : ITagRepository
    {
        public async Task<ExecutionResult> GetAllTagsAsync()
        {
            try
            {
                List<SimpleTag> tags = new();

                using (IAsyncDocumentSession asyncSession = DocumentStoreHolder.Store.OpenAsyncSession())
                {
                    // Получить все теги
                    tags = await asyncSession.Query<SimpleTag>().OrderBy(x => x.Name).ToListAsync();
                }

                return new() { CompletedSuccessfully = true, Data = tags };
            }
            catch (Exception ex)
            {
                return new ExecutionResult() { ErrorText = "Ошибка при работе с базой данных: " + ex.Message };
            }
        }

        public async Task<ExecutionResult> CreateNewTagAsync(SimpleTag tag)
        {
            try
            {
                using (IAsyncDocumentSession asyncSession = DocumentStoreHolder.Store.OpenAsyncSession())
                {
                    // Добавить новый тег в базу данных
                    await asyncSession.StoreAsync(tag);

                    await asyncSession.SaveChangesAsync(); // Сохраняем изменения
                }

                return new() { CompletedSuccessfully = true, Data = tag };
            }
            catch (Exception ex)
            {
                return new ExecutionResult() { ErrorText = "Ошибка при работе с базой данных: " + ex.Message };
            }
        }

        public async Task<ExecutionResult> UpdateTagAsync(SimpleTag tag)
        {
            try
            {
                using (IAsyncDocumentSession asyncSession = DocumentStoreHolder.Store.OpenAsyncSession())
                {
                    // Записать тег в базу данных
                    await asyncSession.StoreAsync(tag);

                    await asyncSession.SaveChangesAsync(); // Сохраняем изменения
                }

                return new() { CompletedSuccessfully = true, Data = tag };
            }
            catch (Exception ex)
            {
                return new ExecutionResult() { ErrorText = "Ошибка при работе с базой данных: " + ex.Message };
            }
        }

        public async Task<ExecutionResult> DeleteTagAsync(SimpleTag tag)
        {
            try
            {
                using (IAsyncDocumentSession asyncSession = DocumentStoreHolder.Store.OpenAsyncSession())
                {
                    // Удалить тег из базы данных
                    asyncSession.Delete(tag.Id);

                    await asyncSession.SaveChangesAsync(); // Сохраняем изменения
                }

                return new() { CompletedSuccessfully = true };
            }
            catch (Exception ex)
            {
                return new ExecutionResult() { ErrorText = "Ошибка при работе с базой данных: " + ex.Message };
            }
        }

        public async Task<ExecutionResult> SetDefaultTagsAsync()
        {
            try
            {
                using (IAsyncDocumentSession asyncSession = DocumentStoreHolder.Store.OpenAsyncSession())
                {
                    // Получить все теги
                    var tags = await asyncSession.Query<SimpleTag>().ToListAsync();

                    // Удалить теги
                    foreach (var document in tags)
                    {
                        asyncSession.Delete(document);
                    }

                    // Добавить новые теги
                    SimpleTag tag = new() { Name = "Баг (ошибка в программе)" };
                    await asyncSession.StoreAsync(tag);

                    tag = new() { Name = "Работа сети" };
                    await asyncSession.StoreAsync(tag);

                    tag = new() { Name = "Права доступа" };
                    await asyncSession.StoreAsync(tag);

                    tag = new() { Name = "Проблема с железом" };
                    await asyncSession.StoreAsync(tag);

                    tag = new() { Name = "Финансовые вопросы" };
                    await asyncSession.StoreAsync(tag);

                    tag = new() { Name = "Разное" };
                    await asyncSession.StoreAsync(tag);

                    await asyncSession.SaveChangesAsync(); // Сохраняем изменения
                }

                return new() { CompletedSuccessfully = true };
            }
            catch (Exception ex)
            {
                return new ExecutionResult() { ErrorText = "Ошибка при работе с базой данных: " + ex.Message };
            }
        }
    }
}

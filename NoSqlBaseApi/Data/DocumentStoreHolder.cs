using Microsoft.Extensions.Configuration;
using Raven.Client.Documents;
using Raven.Client.Documents.Operations.Backups;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Linq;
using static Raven.Client.Constants;

namespace NoSqlBaseApi.Data
{
    public class DocumentStoreHolder()
    {
        private static readonly Lazy<IDocumentStore> _store = new(CreateDocumentStore());

        public static IDocumentStore CreateDocumentStore()
        {
            try
            {
                // Инициализация конфигурации с использованием переменных окружения
                var configuration = new ConfigurationBuilder()
                    .AddEnvironmentVariables() // Читать переменные окружения
                    .Build();

                string serverURL = configuration["RavenDB:ServerUrl"] ?? throw new Exception("Ошибка! В файле 'appsettings.json' не задан параметр 'RavenDB:ServerUrl'");
                string databaseName = configuration["RavenDB:DatabaseName"] ?? throw new Exception("Ошибка! В файле 'appsettings.json' не задан параметр 'RavenDB:ServerUrl'");
                string certificateFileName = configuration["RavenDB:CertificateFileName"] ?? "raven_certificate.pem";

                X509Certificate2 certificate = X509Certificate2.CreateFromPemFile(certificateFileName);
                
                IDocumentStore documentStore = new DocumentStore
                {
                    Urls = new[] { serverURL },
                    Database = databaseName,
                    Certificate = certificate
                };

                documentStore.Initialize();
                return documentStore;
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка при инициализации базы данных: " + ex.Message);
            }           
        }

        public static IDocumentStore Store
        {
            get { return _store.Value; }
        }
    }
}

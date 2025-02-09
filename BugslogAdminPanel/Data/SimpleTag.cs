using System.Text.Json.Serialization;

namespace BugslogAdminPanel.Data
{
    public record SimpleTag
    {
        public string Id { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;
    }
}

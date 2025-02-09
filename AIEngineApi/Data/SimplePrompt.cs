namespace AIEngineApi.Data
{
    public record SimplePrompt
    {
        public string Id { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;

        public bool Default { get; set; } = false;
    }
}

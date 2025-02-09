namespace AIEngineApi.Data
{
    public record ExecutionResult
    {
        public bool CompletedSuccessfully { get; set; } = false;

        public string ErrorText { get; set; } = string.Empty;

        public object? Data { get; set; }
    }
}

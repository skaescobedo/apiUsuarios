namespace apiUsuarios.DTOs.Common
{
    public class ApiErrorResponse
    {
        public string Message { get; set; } = string.Empty;
        public string? Code { get; set; }
        public IDictionary<string, string[]>? Errors { get; set; }
    }
}

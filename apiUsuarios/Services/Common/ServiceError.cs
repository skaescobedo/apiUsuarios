namespace apiUsuarios.Services.Common
{
    public class ServiceError
    {
        public ServiceErrorCode Code { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}

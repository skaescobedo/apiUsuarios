namespace apiUsuarios.Services.Common
{
    public class ServiceResult
    {
        public bool IsSuccess { get; private set; }
        public ServiceError? Error { get; private set; }

        public static ServiceResult Success()
        {
            return new ServiceResult { IsSuccess = true };
        }

        public static ServiceResult Failure(ServiceErrorCode code, string message)
        {
            return new ServiceResult
            {
                IsSuccess = false,
                Error = new ServiceError
                {
                    Code = code,
                    Message = message
                }
            };
        }
    }

    public class ServiceResult<T>
    {
        public bool IsSuccess { get; private set; }
        public T? Value { get; private set; }
        public ServiceError? Error { get; private set; }

        public static ServiceResult<T> Success(T value)
        {
            return new ServiceResult<T>
            {
                IsSuccess = true,
                Value = value
            };
        }

        public static ServiceResult<T> Failure(ServiceErrorCode code, string message)
        {
            return new ServiceResult<T>
            {
                IsSuccess = false,
                Error = new ServiceError
                {
                    Code = code,
                    Message = message
                }
            };
        }
    }
}

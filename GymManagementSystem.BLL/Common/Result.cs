namespace GymManagementSystem.BLL.Common
{
    public class Result
    {
        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public string? ErrorMessage { get; }
        public string? PropertyName { get; }

        protected Result(bool isSuccess, string? errorMessage, string? propertyName = null)
        {
            if (isSuccess && (!string.IsNullOrEmpty(errorMessage) || !string.IsNullOrEmpty(propertyName)))
                throw new InvalidOperationException("Success result cannot have an error message or property name.");

            if (!isSuccess && string.IsNullOrEmpty(errorMessage))
                throw new InvalidOperationException("Failure result must have an error message.");

            IsSuccess = isSuccess;
            ErrorMessage = errorMessage;
            PropertyName = propertyName;
        }

        public static Result Success() => new(true, null);
        public static Result Failure(string errorMessage, string? propertyName = null) => new(false, errorMessage, propertyName);

        public static Result<T> Success<T>(T data) => Result<T>.Success(data);
        public static Result<T> Failure<T>(string errorMessage, string? propertyName = null) => Result<T>.Failure(errorMessage, propertyName);
    }

    public class Result<T> : Result
    {
        public T? Data { get; }

        private Result(bool isSuccess, T? data, string? errorMessage, string? propertyName = null)
            : base(isSuccess, errorMessage, propertyName)
        {
            Data = data;
        }

        public static Result<T> Success(T data) => new(true, data, null);
        public static new Result<T> Failure(string errorMessage, string? propertyName = null) => new(false, default, errorMessage, propertyName);
    }
}

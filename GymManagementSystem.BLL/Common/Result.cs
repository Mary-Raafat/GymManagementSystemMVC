namespace GymManagementSystem.BLL.Common
{
    public class Result
    {
        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public string? ErrorMessage { get; }

        protected Result(bool isSuccess, string? errorMessage)
        {
            if (isSuccess && !string.IsNullOrEmpty(errorMessage))
                throw new InvalidOperationException("Success result cannot have an error message.");

            if (!isSuccess && string.IsNullOrEmpty(errorMessage))
                throw new InvalidOperationException("Failure result must have an error message.");

            IsSuccess = isSuccess;
            ErrorMessage = errorMessage;
        }

        public static Result Success() => new(true, null);
        public static Result Failure(string errorMessage) => new(false, errorMessage);

        public static Result<T> Success<T>(T data) => Result<T>.Success(data);
        public static Result<T> Failure<T>(string errorMessage) => Result<T>.Failure(errorMessage);
    }

    public class Result<T> : Result
    {
        public T? Data { get; }

        private Result(bool isSuccess, T? data, string? errorMessage)
            : base(isSuccess, errorMessage)
        {
            Data = data;
        }

        public static Result<T> Success(T data) => new(true, data, null);
        public static new Result<T> Failure(string errorMessage) => new(false, default, errorMessage);
    }
}

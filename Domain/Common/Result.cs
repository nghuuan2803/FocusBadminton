namespace Domain.Common
{
    public class Result
    {
        public bool Succeeded { get; private set; }
        public List<Error>? Errors { get; private set; }

        protected Result(bool success, List<Error> errors)
        {
            Succeeded = success;
            Errors = errors ?? [];
        }

        public static Result Success() => new(true, null!);
        public static Result Failure(List<Error> errors) => new(false, errors);
        public static Result Failure(Error error) => new(false, new List<Error> { error });
        public static implicit operator Result(Error error) => Failure(error);
    }

    public class Result<T> : Result where T : class
    {
        public T Data { get; private set; }

        private Result(T data, bool success, List<Error> errors) : base(success, errors)
        {
            Data = data;
        }

        public static Result<T> Success(T data) => new(data, true, new List<Error>());
        public static new Result<T> Failure(List<Error> errors) => new(null!, false, errors);
        public static new Result<T> Failure(Error error) => new(null!, false, new List<Error> { error });

        public static implicit operator Result<T>(T data) => Success(data);
        public static implicit operator Result<T>(Error error) => Failure(error);
    }
}

namespace WeatherMonitor.Server.SharedKernel;

public class Result
{
    protected Result(Error? error = null)
    {
        Error = error;
    }

    public Error? Error { get; init; }
    public bool IsSuccess => Error is null;

    public static Result OnSuccess() => new();
    public static Result OnError(Exception exception) => new(error: new Error(exception));
    public static Result OnError(Exception exception, int statusCode) => new(error: new Error(exception, statusCode));
}

public class Error
{
    public Exception Exception { get; }
    public int StatusCode { get; private set; }

    public Error(Exception exception)
    {
        Exception = exception;
        StatusCode = 500;
    }

    public Error(Exception exception, int statusCode)
    {
        Exception = exception;
        StatusCode = statusCode;
    }
}

file class ErrorValueAccessedException : Exception;

public sealed class Result<T> : Result
{
    private Result(
        T? value = default,
        Error? error = null) : base(error)
    {
        _value = value;
    }

    private readonly T? _value;
    public T? Value
    {
        get => _value ?? throw Error?.Exception ?? throw new ErrorValueAccessedException();
        init => _value = value;
    }

    public static Result<T> OnSuccess(T value) => new(value: value);
    public new static Result<T> OnError(Exception exception) => new(error: new Error(exception));
    public new static Result<T> OnError(Exception exception, int statusCode) => new(error: new Error(exception, statusCode));

    public static implicit operator Result<T>(T value) => OnSuccess(value);
    public static implicit operator Result<T>(Exception exception) => OnError(exception);
}

using Microsoft.AspNetCore.Http;

namespace ResponseVacancyEngine.Application.Helpers.ResultPattern;

public class Result
{
    public string? Error { get; private set; }
    public int StatusCode { get; private set; }
    protected bool HasValue => Error == null;
    
    protected Result(int statusCode, string? error = null)
    {
        StatusCode = statusCode;
        Error = error;
    }

    public static implicit operator bool(Result result)
    {
        return result.HasValue;
    }

    public static Result Ok() => new Result(StatusCodes.Status200OK);
    public static Result NoContent() => new Result(StatusCodes.Status204NoContent);
    public static Result NotFound(string error) => new Result(StatusCodes.Status404NotFound, error);
    public static Result BadRequest(string error) => new Result(StatusCodes.Status400BadRequest, error);
    public static Result Fail(int statusCode, string error) => new Result(statusCode, error);
}

public class Result<T> : Result
{
    public T? Data { get; private set; }

    private Result(int statusCode, T? data = default(T)) : base(statusCode)
    {
        Data = data;
    }

    private Result(int statusCode, string error) : base(statusCode, error)
    {
    }
    
    public static Result<T> Ok(T data) => new Result<T>(StatusCodes.Status200OK, data);
    public static Result<T> Ok(string data) => new Result<T>(StatusCodes.Status200OK, data);
    public static Result<T> Created(T data) => new Result<T>(StatusCodes.Status201Created, data);
    public new static Result<T> Ok() => new Result<T>(StatusCodes.Status200OK);
    public new static Result<T> NoContent() => new Result<T>(StatusCodes.Status204NoContent);
    public new static Result<T> BadRequest(string error) => new Result<T>(StatusCodes.Status400BadRequest, error);
    public new static Result<T> Unauthorized(string error) => new Result<T>(StatusCodes.Status401Unauthorized, error);
    public new static Result<T> Forbidden(string error) => new Result<T>(StatusCodes.Status403Forbidden, error);
    public new static Result<T> NotFound(string error) => new Result<T>(StatusCodes.Status404NotFound, error);
    public new static Result<T> Fail(int statusCode, string error) => new Result<T>(statusCode, error);
}

namespace CraqForge.Core.Results;

/// <summary>
/// Representa o resultado de uma operação sem retorno de valor.
/// </summary>
public class Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public string? Error { get; }

    protected Result(bool isSuccess, string? error = null)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    public static Result Success() => new(true);
    public static Result Failure(string error) => new(false, error);
}

/// <summary>
/// Representa o resultado de uma operação, com sucesso ou falha, contendo mensagens e um valor opcional.
/// </summary>
/// <typeparam name="T">Tipo do valor retornado em caso de sucesso.</typeparam>
public class Result<T> : Result
{
    public T? Value { get; }

    protected Result(bool isSuccess, T? value = default, string? error = null) : base(isSuccess, error)
    {
        Value = value;
    }

    public static Result<T> Success(T value) => new(true, value);
    public static new Result<T> Failure(string error) => new(false, default, error);

    public static implicit operator Result<T>(T value) => Success(value);
}

using Unit = LanguageExt.Unit;

namespace HomeInventory.Application.Framework.Messaging;

public static class QueryResult
{
    public static IQueryResult<TResponse> From<TResponse>(TResponse response)
        where TResponse : notnull
        => From((Validated<TResponse, Error>)(response));

    public static IQueryResult<TResponse> From<TResponse>(Validated<TResponse, Error> validation)
        where TResponse : notnull
        => new QueryResult<TResponse>(validation);
}

public sealed class QueryResult<TResponse>(Validated<TResponse, Error> validation) : IQueryResult<TResponse>
    where TResponse : notnull
{
    private readonly Validated<TResponse, Error> _validation = validation;

    public TResponse Success => _validation.ThrowIfFailed(_ => throw new InvalidOperationException("Must be successful"));

    public bool IsFail => _validation.IsFail;

    public bool IsSuccess => _validation.IsSuccess;

    public Error Fail => _validation.ThrowIfSuccess(_ => throw new InvalidOperationException("Must be failed"));

    object IQueryResult.Success => Success;

    public bool Equals(TResponse other) => _validation == other;

    public void IfSuccess(Action<TResponse> onSuccess) => _validation.IfSuccess(onSuccess);

    public TResult Match<TResult>(Func<TResponse, TResult> onSuccess, Func<Error, TResult> onFail) => _validation.Match(onSuccess, onFail);
}

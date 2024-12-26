using Unit = LanguageExt.Unit;

namespace HomeInventory.Application.Framework.Messaging;

public sealed class QueryResult<TResponse>(Validation<Error, TResponse> validation) : IQueryResult<TResponse>
    where TResponse : notnull
{
    private readonly Validation<Error, TResponse> _validation = validation;

    public TResponse Success => (TResponse)_validation;

    public bool IsFail => _validation.IsFail;

    public bool IsSuccess => _validation.IsSuccess;

    public Error Fail => _validation.FailSpan()[0];

    object IQueryResult.Success => Success;

    public bool Equals(TResponse other) => _validation == other;

    public Unit IfSuccess(Action<TResponse> onSuccess) => _validation.Success(onSuccess).Left(static _ => { });

    public TResult Match<TResult>(Func<TResponse, TResult> onSuccess, Func<Error, TResult> onFail) => _validation.Match(onSuccess, onFail);
}
using Unit = LanguageExt.Unit;

namespace HomeInventory.Application.Framework.Messaging;

public interface IQueryResult<TResponse> : IQueryResult
    where TResponse : notnull
{
    new TResponse Success { get; }

    TResult Match<TResult>(Func<TResponse, TResult> onSuccess, Func<Error, TResult> onFail);

    Unit IfSuccess(Action<TResponse> onSuccess);

    bool Equals(TResponse other);
}

public interface IQueryResult
{
    bool IsFail { get; }

    bool IsSuccess { get; }

    Error Fail { get; }

    object Success { get; }
}

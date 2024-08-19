using LanguageExt.Pretty;
using Unit = LanguageExt.Unit;

namespace HomeInventory.Application.Interfaces.Messaging;

public interface IQueryResult<TResponse> : IQueryResult
    where TResponse : notnull
{
    new TResponse Success { get; }

    TResult Match<TResult>(Func<TResponse, TResult> Succ, Func<Seq<Error>, TResult> Fail);

    Unit IfSuccess(Action<TResponse> Success);

    Seq<TResponse> SuccessAsEnumerable();

    bool Equals(TResponse other);
}

public interface IQueryResult
{
    bool IsFail { get; }

    bool IsSuccess { get; }

    Seq<Error> Fail { get; }

    object Success { get; }

    Seq<Error> FailAsEnumerable();
}

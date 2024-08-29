using LanguageExt.Pipes;
using System.Threading;
using Unit = LanguageExt.Unit;

namespace HomeInventory.Domain.Primitives.Messages;

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


public abstract class QueryHandler<TQuery, TResponse> : IRequestHandler<TQuery, IQueryResult<TResponse>>
    where TQuery : IRequestMessage<IQueryResult<TResponse>>
    where TResponse : notnull
{
    protected QueryHandler()
    {
    }

    public async Task<IQueryResult<TResponse>> HandleAsync(IRequestContext<TQuery> context)
    {
        var validation = await InternalHandle(context);
        return new QueryResult(validation);
    }

    protected abstract Task<Validation<Error, TResponse>> InternalHandle(IRequestContext<TQuery> context);

    private sealed class QueryResult(Validation<Error, TResponse> validation) : IQueryResult<TResponse>
    {
        private readonly Validation<Error, TResponse> _validation = validation;

        public TResponse Success => (TResponse)_validation;
        public bool IsFail => _validation.IsFail;
        public bool IsSuccess => _validation.IsSuccess;
        public Seq<Error> Fail => (Seq<Error>)_validation;
        object IQueryResult.Success => Success;

        public bool Equals(TResponse other) => _validation == other;

        public Seq<Error> FailAsEnumerable() => _validation.FailAsEnumerable();

        public LanguageExt.Unit IfSuccess(Action<TResponse> Success) => _validation.IfSuccess(Success);

        public TResult Match<TResult>(Func<TResponse, TResult> Succ, Func<Seq<Error>, TResult> Fail) =>
            _validation.Match(Succ, Fail);

        public Seq<TResponse> SuccessAsEnumerable() => _validation.SuccessAsEnumerable();
    }
}



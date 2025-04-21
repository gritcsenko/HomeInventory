namespace HomeInventory.Application.Framework.Messaging;

public abstract class QueryHandler<TQuery, TResponse> : IQueryHandler<TQuery, TResponse>
    where TQuery : IQuery<TResponse>
    where TResponse : notnull
{
    protected QueryHandler()
    {
    }

    public async Task<IQueryResult<TResponse>> Handle(TQuery request, CancellationToken cancellationToken = default)
    {
        var validation = await InternalHandle(request, cancellationToken);
        return new QueryResult(validation);
    }

    protected abstract Task<Validation<Error, TResponse>> InternalHandle(TQuery query, CancellationToken cancellationToken);

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

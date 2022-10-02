using ErrorOr;
using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace HomeInventory.Application.Authentication.Behaviors;
internal class ValidationPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, ErrorOr<TResponse>>
     where TRequest : IRequest<ErrorOr<TResponse>>
{
    private readonly IReadOnlyCollection<IValidator<TRequest>> _validators;

    public ValidationPipelineBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators.ToArray();
    }

    public async Task<ErrorOr<TResponse>> Handle(TRequest request, RequestHandlerDelegate<ErrorOr<TResponse>> next, CancellationToken cancellationToken)
    {
        if (!_validators.Any())
        {
            return await next();
        }

        var errors = new List<Error>();

        foreach (var validator in _validators)
        {
            var validationResult = await validator.ValidateAsync(request, cancellationToken);
            if (validationResult is not null)
            {
                errors.AddRange(validationResult.Errors.Select(ConvertToError));
            }
        }

        if (!errors.Any())
        {
            return await next();
        }

        return ErrorOr<TResponse>.From(errors);
    }

    private static Error ConvertToError(ValidationFailure failure) => Error.Validation(failure.ErrorCode, failure.ErrorMessage);
}

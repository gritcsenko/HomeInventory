using FluentResults;
using FluentValidation;
using FluentValidation.Results;
using HomeInventory.Domain.Errors;
using MediatR;

namespace HomeInventory.Application.Authentication.Behaviors;
internal class ValidationPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, Result<TResponse>>
     where TRequest : IRequest<Result<TResponse>>
{
    private readonly IReadOnlyCollection<IValidator<TRequest>> _validators;

    public ValidationPipelineBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators.ToArray();
    }

    public async Task<Result<TResponse>> Handle(TRequest request, RequestHandlerDelegate<Result<TResponse>> next, CancellationToken cancellationToken)
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

        return Result.Fail<TResponse>(errors);
    }

    private static Error ConvertToError(ValidationFailure failure) => new ValidationError(failure.ErrorMessage);
}

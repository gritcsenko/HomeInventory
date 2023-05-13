using System.Diagnostics;
using HomeInventory.Domain.Primitives;
using HomeInventory.Domain.Primitives.Errors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;

namespace HomeInventory.Web.Infrastructure;

public class HomeInventoryProblemDetailsFactory : ProblemDetailsFactory
{
    private readonly ApiBehaviorOptions _options;
    private readonly ErrorMapping _errorMapping = new();

    public HomeInventoryProblemDetailsFactory(IOptions<ApiBehaviorOptions> options)
    {
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
    }

    public override ProblemDetails CreateProblemDetails(
        HttpContext httpContext,
        int? statusCode = null,
        string? title = null,
        string? type = null,
        string? detail = null,
        string? instance = null)
    {
        var problemDetails = new ProblemDetails
        {
            Status = statusCode ?? _errorMapping.GetDefaultError(),
            Title = title,
            Type = type,
            Detail = detail,
            Instance = instance,
        };

        ApplyProblemDetailsDefaults(httpContext, problemDetails);

        return problemDetails;
    }

    public override ValidationProblemDetails CreateValidationProblemDetails(
        HttpContext httpContext,
        ModelStateDictionary modelStateDictionary,
        int? statusCode = null,
        string? title = null,
        string? type = null,
        string? detail = null,
        string? instance = null)
    {
        if (modelStateDictionary == null)
        {
            throw new ArgumentNullException(nameof(modelStateDictionary));
        }

        var problemDetails = new ValidationProblemDetails(modelStateDictionary)
        {
            Status = statusCode ?? _errorMapping.GetError<ValidationError>(),
            Type = type,
            Detail = detail,
            Instance = instance,
        };

        if (title != null)
        {
            // For validation problem details, don't overwrite the default title with null.
            problemDetails.Title = title;
        }

        ApplyProblemDetailsDefaults(httpContext, problemDetails);

        return problemDetails;
    }

    public ProblemDetails ConvertToProblem(HttpContext context, IReadOnlyCollection<IError> errors)
    {
        if (errors.Count == 0)
        {
            throw new InvalidOperationException("Has to be at least one error provided");
        }

        context.SetItem(HttpContextItems.Errors, errors);
        if (errors.Count == 1)
        {
            return ConvertToProblem(context, errors.First());
        }

        var statuses = errors.Select(_errorMapping.GetError).ToHashSet();
        var result = CreateProblemDetails(
            context,
            statuses.Count == 1 ? statuses.First() : _errorMapping.GetDefaultError(),
            "Multiple Problems",
            type: null,
            "There were multiple problems that have occurred.",
            instance: null);
        result.Extensions["problems"] = errors.Select(error => ConvertToProblem(context, error)).ToArray();

        return result;
    }

    private ProblemDetails ConvertToProblem(HttpContext context, IError error)
    {
        var result = CreateProblemDetails(
            context,
            _errorMapping.GetError(error),
            error.GetType().Name,
            type: null,
            error.Message,
            instance: null);

        foreach (var pair in error.Metadata)
        {
            result.Extensions[pair.Key] = pair.Value;
        }
        return result;
    }

    private void ApplyProblemDetailsDefaults(HttpContext context, ProblemDetails problemDetails)
    {
        if (_options.ClientErrorMapping.TryGetValue(problemDetails.Status.GetValueOrDefault(), out var clientErrorData))
        {
            problemDetails.Title ??= clientErrorData.Title;
            problemDetails.Type ??= clientErrorData.Link;
        }

        var traceId = Activity.Current?.Id ?? context.TraceIdentifier;
        if (traceId != null)
        {
            problemDetails.Extensions["traceId"] = traceId;
        }

        var errors = context.GetItem(HttpContextItems.Errors);
        if (errors != null)
        {
            problemDetails.Extensions["problems"] = errors.Select(error => ConvertToProblem(context, error)).ToArray();
        }
    }
}

using System.Diagnostics;
using HomeInventory.Core;
using HomeInventory.Domain.Primitives.Errors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;

namespace HomeInventory.Web.Infrastructure;

internal class HomeInventoryProblemDetailsFactory : ProblemDetailsFactory
{
    private readonly ApiBehaviorOptions _options;
    private readonly ErrorMapping _errorMapping;

    public HomeInventoryProblemDetailsFactory(ErrorMapping errorMapping, IOptions<ApiBehaviorOptions> options)
    {
        _errorMapping = errorMapping;
        _options = options.Value;
    }

    public override ProblemDetails CreateProblemDetails(
        HttpContext httpContext,
        int? statusCode = null,
        string? title = null,
        string? type = null,
        string? detail = null,
        string? instance = null)
    {
        var problemDetails = CreateProblem(statusCode, title, type, detail, instance);

        AddProblemDetailsExtensions(httpContext, problemDetails, Array.Empty<IError>());

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

        ApplyProblemDetailsDefaults(problemDetails);

        AddProblemDetailsExtensions(httpContext, problemDetails, Array.Empty<IError>());

        return problemDetails;
    }

    private ProblemDetails CreateProblem(int? statusCode, string? title, string? type, string? detail, string? instance)
    {
        var problemDetails = new ProblemDetails
        {
            Status = statusCode ?? _errorMapping.GetDefaultError(),
            Title = title,
            Type = type,
            Detail = detail,
            Instance = instance,
        };

        ApplyProblemDetailsDefaults(problemDetails);
        return problemDetails;
    }

    public ProblemDetails ConvertToProblem(HttpContext httpContext, IEnumerable<IError> errors)
    {
        var problemDetails = ConvertToProblem(errors);

        AddProblemDetailsExtensions(httpContext, problemDetails, errors);

        return problemDetails;
    }

    private ProblemDetails ConvertToProblem(IEnumerable<IError> errors)
    {
        var problemsAndStatuses = errors.Select(error => (Problem: ConvertToProblem(error), Status: _errorMapping.GetError(error))).ToReadOnly();
        if (problemsAndStatuses.Count == 0)
        {
            throw new InvalidOperationException("Has to be at least one error provided");
        }

        if (problemsAndStatuses.Count == 1)
        {
            return problemsAndStatuses.First().Problem;
        }

        var statuses = problemsAndStatuses.Select(x => x.Status).ToHashSet();
        var status = statuses.Count == 1 ? statuses.First() : _errorMapping.GetDefaultError();
        var problemDetails = CreateProblem(
            status,
            "Multiple Problems",
            type: null,
            "There were multiple problems that have occurred.",
            instance: null);
        problemDetails.Extensions["problems"] = problemsAndStatuses.Select(x => x.Problem).ToArray();

        return problemDetails;
    }

    private ProblemDetails ConvertToProblem(IError error)
    {
        var result = CreateProblem(
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

    private void ApplyProblemDetailsDefaults(ProblemDetails problemDetails)
    {
        if (_options.ClientErrorMapping.TryGetValue(problemDetails.Status.GetValueOrDefault(), out var clientErrorData))
        {
            problemDetails.Title ??= clientErrorData.Title;
            problemDetails.Type ??= clientErrorData.Link;
        }
    }

    private static void AddProblemDetailsExtensions(HttpContext httpContext, ProblemDetails problemDetails, IEnumerable<IError> errors)
    {
        var traceId = Activity.Current?.Id ?? httpContext.TraceIdentifier;
        if (traceId != null)
        {
            problemDetails.Extensions["traceId"] = traceId;
        }

        var errorCodes = errors.Select(e => e.GetType().Name);
        if (errorCodes != null)
        {
            problemDetails.Extensions["errorCodes"] = errorCodes;
        }
    }
}

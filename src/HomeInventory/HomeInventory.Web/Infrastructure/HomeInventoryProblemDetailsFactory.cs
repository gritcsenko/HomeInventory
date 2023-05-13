using System.Diagnostics;
using System.Net;
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
            Status = statusCode ?? StatusCodes.Status500InternalServerError,
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
            Status = statusCode ?? StatusCodes.Status400BadRequest,
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

    public ProblemDetails ConvertToProblem(HttpContext context, IReadOnlyCollection<IError> errors, HttpStatusCode? statusCode = null)
    {
        if (errors.Count == 0)
        {
            throw new InvalidOperationException("Has to be at least one error provided");
        }

        context.SetItem(HttpContextItems.Errors, errors);
        var firstError = errors.First();
        var status = (int)(statusCode ?? GetStatusCode(firstError));
        if (errors.Count == 1)
        {
            return ConvertToProblem(context, firstError, status);
        }

        return new()
        {
            Title = "Multiple Problems",
            Detail = "There were multiple problems that have occurred.",
            Status = status,
            Extensions = {
                ["problems"] = errors.Select(error => ConvertToProblem(context, error, status)).ToArray()
            },
        };
    }

    private ProblemDetails ConvertToProblem(HttpContext context, IError error, int status)
    {
        var result = CreateProblemDetails(
            context,
            status,
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

    private static HttpStatusCode GetStatusCode(IError error) =>
        error switch
        {
            ConflictError => HttpStatusCode.Conflict,
            ValidationError => HttpStatusCode.BadRequest,
            NotFoundError => HttpStatusCode.NotFound,
            _ => HttpStatusCode.InternalServerError,
        };

    private void ApplyProblemDetailsDefaults(HttpContext httpContext, ProblemDetails problemDetails)
    {
        if (_options.ClientErrorMapping.TryGetValue(problemDetails.Status.GetValueOrDefault(), out var clientErrorData))
        {
            problemDetails.Title ??= clientErrorData.Title;
            problemDetails.Type ??= clientErrorData.Link;
        }

        var traceId = Activity.Current?.Id ?? httpContext.TraceIdentifier;
        if (traceId != null)
        {
            problemDetails.Extensions["traceId"] = traceId;
        }

        var errorCodes = httpContext.GetItem(HttpContextItems.Errors)?.Select(e => e.GetType().Name);
        if (errorCodes != null)
        {
            problemDetails.Extensions["errorCodes"] = errorCodes;
        }
    }
}

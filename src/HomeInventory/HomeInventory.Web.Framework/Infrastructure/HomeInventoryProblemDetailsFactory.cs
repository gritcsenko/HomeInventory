using System.Collections.ObjectModel;
using System.Net;
using HomeInventory.Core;
using HomeInventory.Domain.Primitives.Errors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;

namespace HomeInventory.Web.Infrastructure;

internal sealed class HomeInventoryProblemDetailsFactory(ErrorMapping errorMapping, IOptions<ApiBehaviorOptions> options) : ProblemDetailsFactory, IProblemDetailsFactory
{
    private static readonly string? _defaultValidationTitle = new ValidationProblemDetails().Title;
    private readonly ApiBehaviorOptions _options = options.Value;
    private readonly ErrorMapping _errorMapping = errorMapping;
    private readonly int _defaultStatusCode = (int)errorMapping.GetDefaultError();
    private readonly int _defaultValidationStatusCode = (int)errorMapping.GetDefaultValidationError();

    public override ProblemDetails CreateProblemDetails(
        HttpContext httpContext,
        int? statusCode = null,
        string? title = null,
        string? type = null,
        string? detail = null,
        string? instance = null) =>
        CreateProblem<ProblemDetails>(
            statusCode ?? _defaultStatusCode,
            title,
            type,
            detail,
            instance,
            ReadOnlyDictionary<string, object?>.Empty)
            .AddProblemDetailsExtensions(httpContext.TraceIdentifier);

    public override ValidationProblemDetails CreateValidationProblemDetails(
        HttpContext httpContext,
        ModelStateDictionary modelStateDictionary,
        int? statusCode = null,
        string? title = null,
        string? type = null,
        string? detail = null,
        string? instance = null) =>
        CreateProblem<ValidationProblemDetails>(
            statusCode ?? _defaultValidationStatusCode,
            title ?? _defaultValidationTitle,
            type,
            detail,
            instance,
            ReadOnlyDictionary<string, object?>.Empty)
            .ApplyErrors(modelStateDictionary)
            .AddProblemDetailsExtensions(httpContext.TraceIdentifier);

    public ProblemDetails ConvertToProblem(IEnumerable<IError> errors, string? traceIdentifier = null) =>
        InternalConvertToProblem(errors)
        .AddProblemDetailsExtensions(traceIdentifier);

    private ProblemDetails InternalConvertToProblem(IEnumerable<IError> errors)
    {
        var problems = errors.Select(InternalConvertToProblem).ToReadOnly();
        if (problems.Count <= 1)
        {
            return problems.FirstOrDefault() ?? throw new InvalidOperationException("Has to be at least one error provided");
        }

        var statuses = problems.Select(x => x.Status).ToHashSet();
        var status = (statuses.Count == 1 ? statuses.First() : default) ?? _defaultStatusCode;
        return CreateProblem<ProblemDetails>(
            status,
            "Multiple Problems",
            type: null,
            "There were multiple problems that have occurred.",
            instance: null,
            ReadOnlyDictionary<string, object?>.Empty)
            .AddProblemsAndStatuses(problems)
            .AddProblemDetailsExtensions(errors);
    }

    private ProblemDetails InternalConvertToProblem(IError error)
    {
        var errorType = error.GetType();
        var result = CreateProblem<ProblemDetails>(
            _errorMapping.GetError(errorType),
            errorType.Name,
            type: null,
            error.Message,
            instance: null,
            error.Metadata);
        return result;
    }

    private TProblem CreateProblem<TProblem>(HttpStatusCode? statusCode, string? title, string? type, string? detail, string? instance, IReadOnlyDictionary<string, object?> metadata)
        where TProblem : ProblemDetails, new() =>
        CreateProblem<TProblem>((int?)statusCode ?? _defaultStatusCode, title, type, detail, instance, metadata);

    private TProblem CreateProblem<TProblem>(int statusCode, string? title, string? type, string? detail, string? instance, IReadOnlyDictionary<string, object?> metadata)
        where TProblem : ProblemDetails, new() =>
        new TProblem
        {
            Status = statusCode,
            Title = title,
            Type = type,
            Detail = detail,
            Instance = instance,
            Extensions = metadata.ToDictionary(StringComparer.Ordinal)
        }
            .ApplyProblemDetailsDefaults(_options.ClientErrorMapping);
}

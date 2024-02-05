using System.Diagnostics;
using HomeInventory.Domain.Primitives.Errors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace HomeInventory.Web.Infrastructure;

internal static class ProblemDetailsExtensions
{
    public static ValidationProblemDetails ApplyErrors(this ValidationProblemDetails problemDetails, ModelStateDictionary modelStateDictionary)
    {
        problemDetails.Errors = CreateErrorDictionary(modelStateDictionary);
        return problemDetails;
    }

    public static TProblem ApplyProblemDetailsDefaults<TProblem>(this TProblem problemDetails, IDictionary<int, ClientErrorData> clientErrorMapping)
        where TProblem : ProblemDetails
    {
        if (clientErrorMapping.TryGetValue(problemDetails.Status.GetValueOrDefault(), out var clientErrorData))
        {
            problemDetails.Title ??= clientErrorData.Title;
            problemDetails.Type ??= clientErrorData.Link;
        }

        return problemDetails;
    }
    public static TProblem AddProblemDetailsExtensions<TProblem>(this TProblem problemDetails, HttpContext httpContext)
        where TProblem : ProblemDetails =>
        problemDetails.AddProblemDetailsExtensions(httpContext, Array.Empty<IError>());

    public static TProblem AddProblemDetailsExtensions<TProblem>(this TProblem problemDetails, HttpContext httpContext, IEnumerable<IError> errors)
        where TProblem : ProblemDetails
    {
        var traceId = Activity.Current?.Id ?? httpContext.TraceIdentifier;
        if (traceId != null)
        {
            problemDetails.Extensions["traceId"] = traceId;
        }

        problemDetails.Extensions["errorCodes"] = errors.Select(e => e.GetType().Name).ToArray();
        return problemDetails;
    }

    public static TProblem AddProblemsAndStatuses<TProblem>(this TProblem problemDetails, IEnumerable<ProblemDetails> problems)
        where TProblem : ProblemDetails
    {
        problemDetails.Extensions["problems"] = problems.ToArray();
        return problemDetails;
    }

    private static Dictionary<string, string[]> CreateErrorDictionary(ModelStateDictionary modelState)
    {
        var errorDictionary = new Dictionary<string, string[]>(StringComparer.Ordinal);

        foreach (var (key, value) in modelState)
        {
            switch (value.Errors)
            {
                case null:
                case { Count: 0 }:
                    break;
                case { Count: 1 } errors:
                    errorDictionary.Add(key, [errors[0].GetErrorMessage()]);
                    break;
                case { } errors:
                    var errorMessages = new string[errors.Count];
                    for (var i = 0; i < errors.Count; i++)
                    {
                        errorMessages[i] = errors[i].GetErrorMessage();
                    }

                    errorDictionary.Add(key, errorMessages);
                    break;
            }
        }

        return errorDictionary;

    }

    private static string GetErrorMessage(this ModelError error) =>
        string.IsNullOrEmpty(error.ErrorMessage)
            ? "The input was not valid."
            : error.ErrorMessage;
}

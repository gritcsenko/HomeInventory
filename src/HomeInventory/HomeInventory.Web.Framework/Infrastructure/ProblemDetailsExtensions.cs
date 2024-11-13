using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace HomeInventory.Web.Infrastructure;

internal static class ProblemDetailsExtensions
{
    public static ValidationProblemDetails ApplyErrors(this ValidationProblemDetails problemDetails, ModelStateDictionary modelStateDictionary)
    {
        problemDetails.Errors = modelStateDictionary.ToErrorDictionary();
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

    public static TProblem AddProblemDetailsExtensions<TProblem>(this TProblem problemDetails, string? traceIdentifier = null)
        where TProblem : ProblemDetails
    {
        var traceId = Activity.Current?.Id ?? traceIdentifier;
        if (traceId != null)
        {
            problemDetails.Extensions["traceId"] = traceId;
        }

        return problemDetails;
    }

    public static TProblem AddProblemDetailsExtensions<TProblem>(this TProblem problemDetails, Seq<Error> errors)
        where TProblem : ProblemDetails
    {
        problemDetails.Extensions["errorCodes"] = errors.Select(static e => e.GetType().Name).ToArray();
        problemDetails.Extensions["errors"] = errors.ToArray();
        return problemDetails;
    }

    public static TProblem AddProblemsAndStatuses<TProblem>(this TProblem problemDetails, IEnumerable<ProblemDetails> problems)
        where TProblem : ProblemDetails
    {
        problemDetails.Extensions["problems"] = problems.ToArray();
        return problemDetails;
    }

    private static Dictionary<string, string[]> ToErrorDictionary(this ModelStateDictionary modelState) =>
        modelState
            .Select(static p => (p.Key, Messages: p.Value?.Errors.GetErrorMessages() ?? []))
            .Where(static x => x.Messages.Length > 0)
            .ToDictionary(static x => x.Key, static x => x.Messages);

    private static string[] GetErrorMessages(this ModelErrorCollection collection) =>
        collection switch
        {
            { Count: 1 } errors => [errors[0].GetErrorMessage()],
            { } errors => errors.Select(static e => e.GetErrorMessage()).ToArray(),
            _ => [],
        };

    private static string GetErrorMessage(this ModelError error) =>
        string.IsNullOrEmpty(error.ErrorMessage)
            ? "The input was not valid."
            : error.ErrorMessage;
}

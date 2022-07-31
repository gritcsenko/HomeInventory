using System.Text.Json;
using FluentAssertions;
using FluentAssertions.Primitives;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace HomeInventory.Tests.Helpers;
internal static class AssertionExtensions
{
    public static ActionResultAssertions Should(this IActionResult actualValue) => new(actualValue);

    public static ProblemDetailsAssertions Should(this ProblemDetails actualValue) => new(actualValue);

    public static JsonElementAssertions Should(this JsonElement actualValue) => new(actualValue);

    public static AndWhichConstraint<ObjectAssertions, JsonElement> BeJsonElement(this ObjectAssertions assertions) => new(assertions, assertions.BeAssignableTo<JsonElement>().Subject);

    public static ServiceCollectionAssertions Should(this IServiceCollection actualValue) => new(actualValue);
}

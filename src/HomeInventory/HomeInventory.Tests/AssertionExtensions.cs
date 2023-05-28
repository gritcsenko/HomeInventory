using System.Collections;
using System.Text.Json;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace HomeInventory.Tests;
internal static class AssertionExtensions
{
    public static ProblemDetailsAssertions Should(this ProblemDetails actualValue) => new(actualValue);

    public static JsonElementAssertions Should(this JsonElement actualValue) => new(actualValue);

    public static AndWhichConstraint<ObjectAssertions, JsonElement> BeJsonElement(this ObjectAssertions assertions) => new(assertions, assertions.BeAssignableTo<JsonElement>().Subject);

    public static ServiceCollectionAssertions Should(this IServiceCollection actualValue) => new(actualValue);

    public static OkResultAssertions<TValue> Should<TValue>(this Ok<TValue> actualValue) => new(actualValue);

    public static DictionaryAssertions ShouldBeDictionaryAnd(this IDictionary actualValue) => new(actualValue);

    public static OptionAssertions<T> Should<T>(this Optional<T> actualValue)
        where T : notnull =>
        new(actualValue);
}

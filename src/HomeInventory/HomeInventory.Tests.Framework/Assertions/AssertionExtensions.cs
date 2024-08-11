using System.Collections;
using System.Text.Json;
using FluentAssertions.LanguageExt;
using HomeInventory.Application.Interfaces.Messaging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace HomeInventory.Tests.Framework.Assertions;

public static class AssertionExtensions
{
    public static UlidAssertions Should(this Ulid actualValue) => new(actualValue);

    public static ObjectAssertions<ProblemDetails> Should(this ProblemDetails actualValue) => new(actualValue);

    public static JsonElementAssertions Should(this JsonElement actualValue) => new(actualValue);

    public static ServiceCollectionAssertions Should(this IServiceCollection actualValue) => new(actualValue);

    public static OkResultAssertions<TValue> Should<TValue>(this Ok<TValue> actualValue) => new(actualValue);

    public static LanguageExtOptionAssertions<T> Should<T>(this Option<T> actualValue) => new(actualValue);

    public static LanguageExtValidationAssertions<Error, T> Should<T>(this Validation<Error, T> actualValue) => new(actualValue);

    public static QueryResultAssertions<T> Should<T>(this IQueryResult<T> actualValue) where T : notnull => new(actualValue);

    public static GenericCollectionAssertions<EndpointMetadataCollection, object> Should(this EndpointMetadataCollection actualValue) => new(actualValue);

    public static ObjectAssertions<HttpMethodMetadata> Should(this HttpMethodMetadata? actualValue) => new(actualValue);

    public static RouteEndpointAssertions Should(this RouteEndpoint actualValue) => new(actualValue);

    public static DictionaryAssertions ShouldBeDictionaryAnd(this IDictionary actualValue) => new(actualValue);

    public static AndWhichConstraint<ObjectAssertions, JsonElement> BeJsonElement(this ObjectAssertions assertions) =>
        new(assertions, assertions.BeAssignableTo<JsonElement>().Subject);

    public static AndWhichConstraint<GenericCollectionAssertions<RouteEndpoint>, RouteEndpoint> ContainEndpoint(this GenericCollectionAssertions<RouteEndpoint> assertions, string routePattern, string httpMethod) =>
        assertions.ContainSingle(e =>
            e.RoutePattern.RawText == routePattern
            && e.Metadata.OfType<IHttpMethodMetadata>().First().HttpMethods.Contains(httpMethod));
}

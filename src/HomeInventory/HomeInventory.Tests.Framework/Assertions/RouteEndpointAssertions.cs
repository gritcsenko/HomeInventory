using FluentAssertions.Execution;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Patterns;

namespace HomeInventory.Tests.Framework.Assertions;

public sealed class RouteEndpointAssertions(RouteEndpoint value, AssertionChain assertionChain) : ObjectAssertions<RouteEndpoint, RouteEndpointAssertions>(value, assertionChain)
{
    public AndWhichConstraint<RouteEndpointAssertions, RoutePattern> HaveRoutePattern(RoutePattern prefix, RoutePattern expectedPattern) =>
        HaveRoutePattern(RoutePatternFactory.Combine(prefix, expectedPattern));

    public AndWhichConstraint<RouteEndpointAssertions, RoutePattern> HaveRoutePattern(RoutePattern expectedPattern)
    {
        Subject.RoutePattern.RawText.Should().Be(expectedPattern.RawText);
        return new(this, Subject.RoutePattern);
    }
}

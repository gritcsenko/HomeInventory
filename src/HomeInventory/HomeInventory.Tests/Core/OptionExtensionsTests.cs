﻿namespace HomeInventory.Tests.Core;

[UnitTest]
public class OptionExtensionsTests : BaseTest
{
    [Fact]
    public async Task Tap_ShouldInvokeAction_WhenOptionalHasValue()
    {
        Ulid? expected = Ulid.NewUlid();
        var optional = Option<Ulid>.Some(expected.Value);
        var optionalTask = Task.FromResult(optional);

        Ulid? actual = null;
        var tapped = await optionalTask.Tap(x => actual = x);

        using var scope = new AssertionScope();
        tapped.Should().Be(expected.Value);
        actual.Should().Be(expected);
    }

    [Fact]
    public async Task Tap2_ShouldInvokeAction_WhenOptionalHasValue()
    {
        Ulid? expected = Ulid.NewUlid();
        var optional = Option<Ulid>.Some(expected.Value);
        var optionalTask = Task.FromResult(optional);

        Ulid? actual = null;
        var tapped = await optionalTask.Tap(x =>
        {
            actual = x;
            return Task.CompletedTask;
        });

        using var scope = new AssertionScope();
        tapped.Should().Be(expected.Value);
        actual.Should().Be(expected);
    }

    [Fact]
    public async Task Tap3_ShouldInvokeAction_WhenOptionalHasValue()
    {
        Ulid? expected = Ulid.NewUlid();
        var optional = Option<Ulid>.Some(expected.Value);
        var optionalTask = Task.FromResult(optional);

        Ulid? actual = null;
        var tapped = await optionalTask.Tap(x =>
        {
            actual = x;
            return ValueTask.CompletedTask;
        });

        using var scope = new AssertionScope();
        tapped.Should().Be(expected.Value);
        actual.Should().Be(expected);
    }
}

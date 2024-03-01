using NetArchTest.Rules;

namespace HomeInventory.Tests.Architecture;

internal static class Namespaces
{
    private const string _prefix = "HomeInventory.";
    public const string Core = _prefix + nameof(Core);
    public const string Domain = _prefix + nameof(Domain);
    public const string DomainPrimitives = _prefix + nameof(Domain) + ".Primitives";
    public const string Application = _prefix + nameof(Application);
    public const string Infrastructure = _prefix + nameof(Infrastructure);
    public const string Api = _prefix + nameof(Api);
    public const string Web = _prefix + nameof(Web);
    public const string Contracts = _prefix + nameof(Contracts);
    public const string ContractsValidation = Contracts + ".Validation";
    public const string MediatR = nameof(MediatR);
    public const string AutoMapper = nameof(AutoMapper);
    public static IEnumerable<string> HomeInventory = new[] { Domain, Application, Infrastructure, Api, Web, Contracts, ContractsValidation };
}

[ArchitectureTest]
public class ArchitectureTests
{
    [Theory]
    [InlineData(typeof(HomeInventory.Domain.AssemblyReference), new[] { Namespaces.DomainPrimitives })]
    [InlineData(typeof(HomeInventory.Domain.Primitives.AssemblyReference), new[] { Namespaces.Core })]
    [InlineData(typeof(HomeInventory.Core.AssemblyReference), new string[0])]
    [InlineData(typeof(HomeInventory.Application.AssemblyReference), new[] { Namespaces.Domain })]
    [InlineData(typeof(Infrastructure.AssemblyReference), new[] { Namespaces.Application })]
    [InlineData(typeof(Contracts.AssemblyReference), new string[0])]
    [InlineData(typeof(Contracts.UserManagement.AssemblyReference), new string[0])]
    [InlineData(typeof(Contracts.Validations.AssemblyReference), new[] { Namespaces.Contracts })]
    [InlineData(typeof(Web.UserManagement.AssemblyReference), new[] { Namespaces.Application, Namespaces.ContractsValidation })]
    [InlineData(typeof(Api.AssemblyReference), new[] { Namespaces.Web, Namespaces.Infrastructure })]
    public void Should_NotHaveBadDependencies(Type assemblyMarkerType, IEnumerable<string> allowed)
    {
        var assembly = assemblyMarkerType.Assembly;
        var otherProjects = Namespaces.HomeInventory.Except(allowed.Concat(assemblyMarkerType.Namespace)).ToArray();

        var result = Types.InAssembly(assembly)
            .Should()
            .NotHaveDependencyOnAll(otherProjects)
            .GetResult();

        result.FailingTypeNames.Should().BeNullOrEmpty();
    }

    [Fact]
    public void CommandHandlers_Should_HaveDependencyOn_Domain()
    {
        var assembly = HomeInventory.Application.AssemblyReference.Assembly;

        var result = Types.InAssembly(assembly)
            .That()
            .HaveNameEndingWith("CommandHandler", StringComparison.Ordinal)
            .Should()
            .HaveDependencyOn(Namespaces.Domain)
            .GetResult();

        result.FailingTypeNames.Should().BeNullOrEmpty();
    }

    [Fact]
    public void QueryHandlers_Should_HaveDependencyOn_Domain()
    {
        var assembly = HomeInventory.Application.AssemblyReference.Assembly;

        var result = Types.InAssembly(assembly)
            .That()
            .HaveNameEndingWith("QueryHandler", StringComparison.Ordinal)
            .Should()
            .HaveDependencyOn(Namespaces.Domain)
            .GetResult();

        result.FailingTypeNames.Should().BeNullOrEmpty();
    }

    [Fact]
    public void Controllers_Should_HaveDependencyOn_MediatR()
    {
        var assembly = Web.UserManagement.AssemblyReference.Assembly;

        var result = Types.InAssembly(assembly)
            .That()
            .HaveNameEndingWith("Controller", StringComparison.Ordinal)
            .And()
            .AreNotAbstract()
            .Should()
            .HaveDependencyOn(Namespaces.MediatR)
            .GetResult();

        result.FailingTypeNames.Should().BeNullOrEmpty();
    }

    [Fact]
    public void Controllers_Should_HaveDependencyOn_Automapper()
    {
        var assembly = Web.UserManagement.AssemblyReference.Assembly;

        var result = Types.InAssembly(assembly)
            .That()
            .HaveNameEndingWith("Controller", StringComparison.Ordinal)
            .And()
            .AreNotAbstract()
            .Should()
            .HaveDependencyOn(Namespaces.AutoMapper)
            .GetResult();

        result.FailingTypeNames.Should().BeNullOrEmpty();
    }
}

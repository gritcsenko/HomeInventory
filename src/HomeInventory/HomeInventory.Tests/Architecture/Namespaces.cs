using NetArchTest.Rules;

namespace HomeInventory.Tests.Architecture;

internal static class Namespaces
{
    private const string Prefix = "HomeInventory.";
    public const string Domain = Prefix + nameof(Domain);
    public const string Application = Prefix + nameof(Application);
    public const string Infrastructure = Prefix + nameof(Infrastructure);
    public const string Api = Prefix + nameof(Api);
    public const string Web = Prefix + nameof(Web);
    public const string Contracts = Prefix + nameof(Contracts);
    public const string ContractsValidation = Contracts + ".Validation";
    public const string MediatR = nameof(MediatR);
    public const string AutoMapper = nameof(AutoMapper);
    public static IEnumerable<string> HomeInventory = new[] { Domain, Application, Infrastructure, Api, Web, Contracts, ContractsValidation };
}

[ArchitectureTest]
public class ArchitectureTests
{
    [Theory]
    [InlineData(typeof(HomeInventory.Domain.AssemblyReference), new string[0])]
    [InlineData(typeof(Application.AssemblyReference), new[] { Namespaces.Domain })]
    [InlineData(typeof(Infrastructure.AssemblyReference), new[] { Namespaces.Application })]
    [InlineData(typeof(Contracts.AssemblyReference), new string[0])]
    [InlineData(typeof(Contracts.Validations.AssemblyReference), new[] { Namespaces.Contracts })]
    [InlineData(typeof(Web.AssemblyReference), new[] { Namespaces.Application, Namespaces.ContractsValidation })]
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
        var assembly = Application.AssemblyReference.Assembly;

        var result = Types.InAssembly(assembly)
            .That()
            .HaveNameEndingWith("CommandHandler")
            .Should()
            .HaveDependencyOn(Namespaces.Domain)
            .GetResult();

        result.FailingTypeNames.Should().BeNullOrEmpty();
    }

    [Fact]
    public void QueryHandlers_Should_HaveDependencyOn_Domain()
    {
        var assembly = Application.AssemblyReference.Assembly;

        var result = Types.InAssembly(assembly)
            .That()
            .HaveNameEndingWith("QueryHandler")
            .Should()
            .HaveDependencyOn(Namespaces.Domain)
            .GetResult();

        result.FailingTypeNames.Should().BeNullOrEmpty();
    }

    [Fact]
    public void Controllers_Should_HaveDependencyOn_MediatR()
    {
        var assembly = Web.AssemblyReference.Assembly;

        var result = Types.InAssembly(assembly)
            .That()
            .HaveNameEndingWith("Controller")
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
        var assembly = Web.AssemblyReference.Assembly;

        var result = Types.InAssembly(assembly)
            .That()
            .HaveNameEndingWith("Controller")
            .And()
            .AreNotAbstract()
            .Should()
            .HaveDependencyOn(Namespaces.AutoMapper)
            .GetResult();

        result.FailingTypeNames.Should().BeNullOrEmpty();
    }
}

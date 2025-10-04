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
    public static IEnumerable<string> HomeInventory = [Domain, Application, Infrastructure, Api, Web, Contracts, ContractsValidation];
}

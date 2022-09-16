using ErrorOr;

namespace HomeInventory.Domain;
public static partial class Errors
{
    public static class Domain
    {
        public static Error ValidatorNotFound { get; } = Error.NotFound($"{nameof(Domain)}.{nameof(ValidatorNotFound)}", "Validator not found");
    }
}

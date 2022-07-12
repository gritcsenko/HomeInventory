using ErrorOr;

namespace HomeInventory.Domain;
public static partial class Errors
{
    public static class User
    {
        public static Error DuplicateEmail = Error.Conflict($"{nameof(User)}.{nameof(DuplicateEmail)}", "Duplicate email");
        public static Error UserIdCreation = Error.Failure($"{nameof(User)}.{nameof(UserIdCreation)}", "Failed to create new user id");
    }
}

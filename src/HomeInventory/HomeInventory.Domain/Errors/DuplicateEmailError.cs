namespace HomeInventory.Domain.Errors;

public class DuplicateEmailError : ConflictError
{
    public DuplicateEmailError()
        : base("Duplicate email")
    {
    }
}

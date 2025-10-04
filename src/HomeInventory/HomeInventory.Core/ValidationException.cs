namespace HomeInventory.Core;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1032:Implement standard exception constructors", Justification = "By Design")]
public sealed class ValidationException(ValidationError error) : ExceptionalException(error.Message, error.Code)
{
    public object? Value { get; } = error.Value;
}

namespace HomeInventory.Domain.Primitives.Errors;

public record class ValueNotSpecifiedError() : Exceptional("Value was not specified", -1_000_000_004)
{
}

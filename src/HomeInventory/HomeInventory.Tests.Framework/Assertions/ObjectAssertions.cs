namespace HomeInventory.Tests.Framework.Assertions;

public class ObjectAssertions<T>(T? value) : ObjectAssertions<T, ObjectAssertions<T>>(value!)
{
}

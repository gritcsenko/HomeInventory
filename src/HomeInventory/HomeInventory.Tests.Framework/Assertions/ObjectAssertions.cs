namespace HomeInventory.Tests.Framework.Assertions;

public class ObjectAssertions<T>(T? value, AssertionChain assertionChain) : ObjectAssertions<T, ObjectAssertions<T>>(value!, assertionChain)
{
}

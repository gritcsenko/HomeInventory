using CollectionExtensions = HomeInventory.Core.CollectionExtensions;

namespace HomeInventory.Tests.Core;

[UnitTest]
public sealed class CollectionExtensionsTests : BaseTest
{
    [Fact]
    public void AddRange_ShouldNotCallAdd_WhenListIsSupplied()
    {
        // Given
        var collection = new CollectionSubject<Guid>();

        CollectionExtensions.AddRange(collection, [Fixture.Create<Guid>()]);

        collection.IsAddCalled.Should().BeFalse();
    }

    [Fact]
    public void AddRange_ShouldCallAdd()
    {
        // Given
        var collection = Substitute.For<ICollection<Guid>>();
        var item = Fixture.Create<Guid>();

        CollectionExtensions.AddRange(collection, [item]);

        collection.Received(1).Add(item);
    }

    private sealed class CollectionSubject<T> : List<T>, ICollection<T>
    {
        public bool IsAddCalled { get; private set; }

        bool ICollection<T>.IsReadOnly => false;

        void ICollection<T>.Add(T item) => IsAddCalled = true;
    }
}

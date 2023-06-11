using Microsoft.Extensions.DependencyInjection;

namespace HomeInventory.Tests.Framework.Assertions;

[UnitTest]
public class ServiceDescriptorExtensionsTests : BaseTest
{
    private readonly IServiceProvider _serviceProvider = Substitute.For<IServiceProvider>();

    public ServiceDescriptorExtensionsTests()
    {
    }

    [Fact]
    public void GetInstance_ShouldReturnFromImplementationInstance()
    {
        var expected = Fixture.Create<object>();
        var descriptor = ServiceDescriptor.Singleton(expected.GetType(), expected);

        var actual = ServiceDescriptorExtensions.GetInstance(descriptor, _serviceProvider);

        actual.Should().BeSameAs(expected);
    }

    [Fact]
    public void GetInstance_ShouldReturnFromImplementationFactory()
    {
        var expected = Fixture.Create<object>();
        var descriptor = ServiceDescriptor.Singleton(expected.GetType(), sp =>
        {
            sp.Should().BeSameAs(_serviceProvider);
            return expected;
        });

        var actual = ServiceDescriptorExtensions.GetInstance(descriptor, _serviceProvider);

        actual.Should().BeSameAs(expected);
    }

    [Fact]
    public void GetInstance_ShouldReturnFromImplementationType()
    {
        var expectedType = typeof(object);
        var descriptor = ServiceDescriptor.Singleton(typeof(object), expectedType);

        var actual = ServiceDescriptorExtensions.GetInstance(descriptor, _serviceProvider);

        actual.Should().BeAssignableTo(expectedType);
    }
}

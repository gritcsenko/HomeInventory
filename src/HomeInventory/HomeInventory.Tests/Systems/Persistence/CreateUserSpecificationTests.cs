using HomeInventory.Application.Interfaces.Persistence.Specifications;
using HomeInventory.Domain.ValueObjects;

namespace HomeInventory.Tests.Systems.Persistence;

[UnitTest]
public class CreateUserSpecificationTests : BaseTest
{
    [Fact]
    public void Should_PreserveArguments()
    {
        var email = new Email(Fixture.Create<string>());
        var password = Fixture.Create<string>();
        var supplier = Substitute.For<ISupplier<Guid>>();

        var sut = new CreateUserSpecification(email, password, supplier);

        sut.Email.Should().Be(email);
        sut.Password.Should().Be(password);
        sut.UserIdSupplier.Should().BeSameAs(supplier);
    }
}

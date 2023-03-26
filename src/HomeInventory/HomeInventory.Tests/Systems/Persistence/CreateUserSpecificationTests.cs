using HomeInventory.Application.Interfaces.Persistence.Specifications;
using HomeInventory.Domain.ValueObjects;

namespace HomeInventory.Tests.Systems.Persistence;

[Trait("Category", "Unit")]
public class CreateUserSpecificationTests : BaseTest
{
    [Fact]
    public void Should_PreserveArguments()
    {
        var firstName = Fixture.Create<string>();
        var lastName = Fixture.Create<string>();
        var email = new Email(Fixture.Create<string>());
        var password = Fixture.Create<string>();

        var sut = new CreateUserSpecification(firstName, lastName, email, password);

        sut.FirstName.Should().Be(firstName);
        sut.LastName.Should().Be(lastName);
        sut.Email.Should().Be(email);
        sut.Password.Should().Be(password);
    }
}

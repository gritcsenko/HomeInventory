using FluentValidation;
using HomeInventory.Web.Framework;
using Microsoft.Extensions.Options;

namespace HomeInventory.Tests.Presentation.Web;

[UnitTest]
public sealed class FluentOptionsValidatorTests() : BaseTest<FluentOptionsValidatorTestsGivenContext>(static t => new(t))
{
    [Fact]
    public void Create_Should_ReturnValidator()
    {
        Given
            .New<string>(out var nameVar)
            .SubstituteFor<IValidator>(out var validatorVar);

        var then = When
            .Invoked(nameVar, validatorVar, static (name, validator) => FluentOptionsValidator.Create<SubjectOptions>(name, validator));

        then.Result(actual => actual.Should().BeAssignableTo<FluentOptionsValidator<SubjectOptions>>());
    }

    [Fact]
    public void Validate_Should_Skip_When_NameIsDifferent()
    {
        Given
            .New<string>(out var nameVar)
            .New<string>(out var differentNameVar)
            .New<SubjectOptions>(out var optionsVar, () => new())
            .SubstituteFor<IValidator>(out var validatorVar)
            .New(out var sutVar, nameVar, validatorVar, static (name, validator) => FluentOptionsValidator.Create<SubjectOptions>(name, validator));

        var then = When
            .Invoked(sutVar, differentNameVar, optionsVar, (sut, name, options) => sut.Validate(name, options));

        then.Result(actual => actual.Should().BeSameAs(ValidateOptionsResult.Skip));
    }

    [Fact]
    public void Validate_Should_CallValidator()
    {
        Given
            .New<string>(out var nameVar)
            .New<SubjectOptions>(out var optionsVar, () => new())
            .New<FluentValidation.Results.ValidationResult>(out var resultVar, () => new())
            .SubstituteFor<IValidationContext>(out var validationContextVar)
            .SubstituteFor(out IVariable<IValidationContextFactory<SubjectOptions>> factoryVar, optionsVar, validationContextVar, static (f, o, ctx) => f.CreateContext(o).Returns(ctx))
            .SubstituteFor(out IVariable<IValidator> validatorVar, validationContextVar, resultVar, static (v, ctx, r) => v.Validate(ctx).Returns(r))
            .New(out var sutVar, nameVar, validatorVar, factoryVar, static (name, validator, factory) => FluentOptionsValidator.Create(name, validator, factory));

        var then = When
            .Invoked(sutVar, nameVar, optionsVar, (sut, name, options) => sut.Validate(name, options));

        then
            .Result(actual => actual.Should().BeSameAs(ValidateOptionsResult.Success))
            .Ensure(validatorVar, validationContextVar, static (validator, context) => validator.Received(1).Validate(context));
    }
}

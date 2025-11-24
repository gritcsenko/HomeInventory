using System.Diagnostics.CodeAnalysis;
using HomeInventory.Domain.Errors;
using HomeInventory.Domain.Primitives.Errors;
using HomeInventory.Web.ErrorHandling;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace HomeInventory.Tests.Systems.ErrorHandling;

[UnitTest]
[SuppressMessage("ReSharper", "UnusedType.Global")]
public class ProblemDetailsExtensionsTests() : BaseTest<ProblemDetailsExtensionsTestsGivenContext>(static t => new(t))
{
    [Fact]
    public void ApplyErrors_WithModelStateDictionary_AddsErrors()
    {
        Given
            .New<ValidationProblemDetails>(out var problemDetailsVar)
            .ModelStateDictionaryWithTwoFields(out var modelStateVar, out var field1Var, out var field2Var);

        var then = When
            .Invoked(problemDetailsVar, modelStateVar, static (pd, ms) => pd.ApplyErrors(ms));

        then
            .Result(field1Var, field2Var, static (result, f1, f2) =>
            {
                result.Errors.Should().HaveCount(2);
                result.Errors.Should().ContainKey(f1);
                result.Errors.Should().ContainKey(f2);
            });
    }

    [Fact]
    public void AddProblemDetailsExtensions_WithErrors_AddsErrorCodesAndErrors()
    {
        Given
            .New<ProblemDetails>(out var problemDetailsVar)
            .New<string>(out var errorMessageVar)
            .New(out var errorsVar, errorMessageVar, static msg =>
            new List<Error> {
                InvalidCredentialsError.Instance,
                new NotFoundError(msg),
            });

        var then = When
            .Invoked(problemDetailsVar, errorsVar, static (pd, errors) => pd.AddProblemDetailsExtensions(errors));

        then
            .Result(static result =>
            {
                result.Extensions.Should().ContainKey("errorCodes")
                    .WhoseValue.Should().BeAssignableTo<string[]>()
                    .Which.Should().Contain("InvalidCredentialsError")
                    .And.Contain("NotFoundError");
                result.Extensions.Should().ContainKey("errors")
                    .WhoseValue.Should().BeAssignableTo<Error[]>()
                    .Which.Should().HaveCount(2);
            });
    }

    [Fact]
    public void AddProblemsAndStatuses_WithProblems_AddsProblemsExtension()
    {
        Given
            .New<ProblemDetails>(out var problemDetailsVar)
            .New<List<ProblemDetails>>(out var problemsVar);

        var then = When
            .Invoked(problemDetailsVar, problemsVar, static (pd, problems) => pd.AddProblemsAndStatuses(problems));

        then
            .Result(static result =>
                result.Extensions.Should().ContainKey("problems")
                    .WhoseValue.Should().BeAssignableTo<ProblemDetails[]>()
                    .Which.Should().NotBeEmpty());
    }

    [Fact]
    public void ApplyErrors_WithEmptyModelState_AddsEmptyErrors()
    {
        Given
            .New<ValidationProblemDetails>(out var problemDetailsVar)
            .New<ModelStateDictionary>(out var modelStateVar);

        var then = When
            .Invoked(problemDetailsVar, modelStateVar, static (pd, ms) => pd.ApplyErrors(ms));

        then
            .Result(static result => result.Errors.Should().BeEmpty());
    }

    [Fact]
    public void ApplyErrors_WithModelErrorWithoutMessage_UsesDefaultMessage()
    {
        Given
            .New<ValidationProblemDetails>(out var problemDetailsVar)
            .New<string>(out var fieldVar)
            .New(out var modelStateVar, fieldVar, static field =>
            {
                var ms = new ModelStateDictionary();
                ms.AddModelError(field, string.Empty);
                return ms;
            });

        var then = When
            .Invoked(problemDetailsVar, modelStateVar, static (pd, ms) => pd.ApplyErrors(ms));

        then
            .Result(fieldVar, static (result, field) =>
                result.Errors.Should().ContainKey(field)
                    .WhoseValue.Should().Contain("The input was not valid."));
    }
}

public sealed class ProblemDetailsExtensionsTestsGivenContext(BaseTest test) : GivenContext<ProblemDetailsExtensionsTestsGivenContext>(test)
{
    public ProblemDetailsExtensionsTestsGivenContext ModelStateDictionaryWithTwoFields(
        out IVariable<ModelStateDictionary> modelStateVar,
        out IVariable<string> field1Var,
        out IVariable<string> field2Var)
    {
        New(out field1Var);
        New(out field2Var);
        New(out modelStateVar, field1Var, field2Var, (f1, f2) =>
        {
            var ms = new ModelStateDictionary();
            ms.AddModelError(f1, Create<string>());
            ms.AddModelError(f2, Create<string>());
            return ms;
        });
        return This;
    }
}


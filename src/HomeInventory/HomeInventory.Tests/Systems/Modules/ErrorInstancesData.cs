using HomeInventory.Domain.Errors;
using HomeInventory.Domain.Primitives.Errors;
using Microsoft.AspNetCore.Http;

namespace HomeInventory.Tests.Systems.Modules;

public sealed class ErrorInstancesData : TheoryData<IError, int>
{
    public ErrorInstancesData()
    {
        Add(new ConflictError(""), StatusCodes.Status409Conflict);
        Add(new DuplicateEmailError(), StatusCodes.Status409Conflict);
        Add(new ValidationError(""), StatusCodes.Status400BadRequest);
        Add(new ObjectValidationError<string>(""), StatusCodes.Status400BadRequest);
        Add(new NotFoundError(""), StatusCodes.Status404NotFound);
    }
}

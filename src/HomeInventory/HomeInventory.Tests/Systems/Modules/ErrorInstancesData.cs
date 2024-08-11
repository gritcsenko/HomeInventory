﻿using System.Net;
using HomeInventory.Domain.Errors;
using HomeInventory.Domain.Primitives.Errors;

namespace HomeInventory.Tests.Systems.Modules;

public sealed class ErrorInstancesData : TheoryData<Type?, HttpStatusCode>
{
    public ErrorInstancesData()
    {
        Add(null, HttpStatusCode.InternalServerError);
        Add(typeof(ConflictError), HttpStatusCode.Conflict);
        Add(typeof(DuplicateEmailError), HttpStatusCode.Conflict);
        Add(typeof(ValidationError), HttpStatusCode.BadRequest);
        Add(typeof(NotFoundError), HttpStatusCode.NotFound);
    }
}

﻿using System.Net;
using HomeInventory.Domain.Errors;
using HomeInventory.Domain.Primitives.Errors;

namespace HomeInventory.Web.Infrastructure;

internal sealed class ErrorMappingBuilder
{
    private readonly HttpStatusCode _default;
    private readonly HttpStatusCode _defaultValidation;
    private readonly Dictionary<Type, HttpStatusCode> _mapping;

    private ErrorMappingBuilder(HttpStatusCode @default, HttpStatusCode validation, IEnumerable<KeyValuePair<Type, HttpStatusCode>> mapping)
    {
        _default = @default;
        _defaultValidation = validation;
        _mapping = new Dictionary<Type, HttpStatusCode>(mapping);
    }

    public ErrorMappingBuilder Add<TError>(HttpStatusCode statusCode) => new(_default, _defaultValidation, _mapping.Concat(MapError<TError>(statusCode)));

    public ErrorMapping Build() => new(_default, _defaultValidation, _mapping);

    public static ErrorMappingBuilder CreateDefault() =>
        new ErrorMappingBuilder(HttpStatusCode.InternalServerError, HttpStatusCode.BadRequest, [])
            .Add<ConflictError>(HttpStatusCode.Conflict)
            .Add<ValidationError>(HttpStatusCode.BadRequest)
            .Add<NotFoundError>(HttpStatusCode.NotFound)
            .Add<InvalidCredentialsError>(HttpStatusCode.Forbidden);

    private static KeyValuePair<Type, HttpStatusCode> MapError<TError>(HttpStatusCode statusCode) => KeyValuePair.Create(typeof(TError), statusCode);
}

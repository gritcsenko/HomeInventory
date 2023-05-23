using System.Runtime.Versioning;
using DotNext;
using HomeInventory.Domain.Primitives;

namespace HomeInventory.Domain.ValueObjects;

public sealed class Email : ValueObject<Email>, IBuildable<Email, ValueObject<Email>.Builder<string>>
{
    internal Email(string value)
        : base(value)
    {
        Value = value;
    }

    public string Value { get; }

    [RequiresPreviewFeatures]
    public static Builder<string> CreateBuilder() => new(value => new Email(value));
}

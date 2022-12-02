using AutoMapper;
using HomeInventory.Domain.ValueObjects;

namespace HomeInventory.Application.Mapping;

public class EmailConverter : IValueConverter<string, Email>, ITypeConverter<string, Email>
{
    private readonly IEmailFactory _factory;

    public EmailConverter(IEmailFactory factory) => _factory = factory;

    public Email Convert(string sourceMember, ResolutionContext context) => (Email)_factory.CreateFrom(sourceMember).Value;

    public Email Convert(string source, Email destination, ResolutionContext context) => (Email)_factory.CreateFrom(source).Value;
}

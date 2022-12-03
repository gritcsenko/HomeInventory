using HomeInventory.Domain.ValueObjects;

namespace HomeInventory.Application.Mapping;

public class EmailConverter : ValueObjectConverter<Email, string>
{
    public EmailConverter(IEmailFactory factory)
        : base(factory)
    {
    }
}

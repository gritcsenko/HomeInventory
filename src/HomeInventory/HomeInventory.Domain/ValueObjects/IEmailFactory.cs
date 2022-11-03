using FluentResults;

namespace HomeInventory.Domain.ValueObjects;

public interface IEmailFactory
{
    IResult<Email> CreateFrom(string value);
}

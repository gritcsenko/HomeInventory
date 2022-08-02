namespace HomeInventory.Contracts.Validations;

internal class CompositePasswordCharacterSet : IPasswordCharacterSet
{
    private readonly IEnumerable<IPasswordCharacterSet> _sets;

    public CompositePasswordCharacterSet(IEnumerable<IPasswordCharacterSet> sets) => _sets = sets;

    public CompositePasswordCharacterSet(params IPasswordCharacterSet[] sets) => _sets = sets;

    public bool IsEmpty => _sets.All(s => s.IsEmpty);

    public bool ContainsAll(IEnumerable<char> characters) => _sets.Any(s => s.ContainsAll(characters));
}

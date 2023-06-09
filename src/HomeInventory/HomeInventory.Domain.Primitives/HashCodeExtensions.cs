namespace HomeInventory.Domain.Primitives;

internal static class HashCodeExtensions
{
    public static HashCode Combine(this HashCode hash, object obj)
    {
        hash.Add(obj);
        return hash;
    }
}

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace HomeInventory.Tests.Framework;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA5394:Do not use insecure randomness", Justification = "By dedign")]
public static class RandomExtensions
{
    public static Option<T> Peek<T>(this Random random, IReadOnlyCollection<T> collection)
    {
        return collection switch
        {
            null => throw new ArgumentNullException(nameof(collection)),
            { Count: 0 } => Option<T>.None,
            { Count: 1 } => collection.ElementAt(0),
            ISpannableCollection<T> s => random.Peek<T>(s.AsSpan()),
            T[] array => random.Peek<T>(array.AsSpan()),
            List<T> list => random.Peek<T>(CollectionsMarshal.AsSpan(list)),
            _ => PeekRandomSlow(random, collection),
        };

        [MethodImpl(MethodImplOptions.NoInlining)]
        static Option<T> PeekRandomSlow(Random random, IReadOnlyCollection<T> collection)
        {
            var index = random.Next(collection.Count);
            using var enumerator = collection.GetEnumerator();
#pragma warning disable S1994 // "for" loop increment clauses should modify the loops' counters
            for (var i = 0; enumerator.MoveNext(); i++)
            {
                if (i == index)
                {
                    return enumerator.Current;
                }
            }
#pragma warning restore S1994 // "for" loop increment clauses should modify the loops' counters

            return Option<T>.None;
        }
    }

    public static Option<T> Peek<T>(this Random random, ReadOnlySpan<T> span) =>
        span.Length switch
        {
            0 => Option<T>.None,
            1 => MemoryMarshal.GetReference(span),
            var l => span[random.Next(l)],
        };
}

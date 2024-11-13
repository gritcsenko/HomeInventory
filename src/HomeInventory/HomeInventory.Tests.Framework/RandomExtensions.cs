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
            { Count: 0 } => OptionNone.Default,
            { Count: 1 } => collection.ElementAt(0),
            ISpannableCollection<T> spannable => random.Peek<T>(spannable.AsSpan()),
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
                    return enumerator.Current;
            }
#pragma warning restore S1994 // "for" loop increment clauses should modify the loops' counters

            return OptionNone.Default;
        }
    }

    public static Option<T> Peek<T>(this Random random, ReadOnlySpan<T> span)
    {
        var length = span.Length;

        return length switch
        {
            0 => OptionNone.Default,
            1 => MemoryMarshal.GetReference(span),
            _ => span[random.Next(length)],
        };
    }
}

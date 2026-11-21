using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace HomeInventory.Tests.Framework;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA5394:Do not use insecure randomness", Justification = "By dedign")]
public static class RandomExtensions
{
    extension(Random random)
    {
        public Optional<T> Peek<T>(IReadOnlyCollection<T> collection)
        {
            return collection switch
            {
                null => throw new ArgumentNullException(nameof(collection)),
                { Count: 0 } => None.Value,
                { Count: 1 } => collection.ElementAt(0),
                ISpannableCollection<T> s => random.Peek(s.AsSpan()),
                T[] array => random.Peek(array.AsSpan()),
                List<T> list => random.Peek(CollectionsMarshal.AsSpan(list)),
                _ => PeekRandomSlow(random, collection),
            };

            [MethodImpl(MethodImplOptions.NoInlining)]
            static Optional<T> PeekRandomSlow(Random random, IReadOnlyCollection<T> collection)
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

                return None.Value;
            }
        }

        public Optional<T> Peek<T>(ReadOnlySpan<T> span) =>
            span.Length switch
            {
                0 => Optional<T>.None,
                1 => MemoryMarshal.GetReference(span),
                var l => span[random.Next(l)],
            };
    }
}

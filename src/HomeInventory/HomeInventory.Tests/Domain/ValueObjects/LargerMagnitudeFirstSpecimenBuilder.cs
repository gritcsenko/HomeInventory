using System.Diagnostics.CodeAnalysis;
using AutoFixture.Kernel;

namespace HomeInventory.Tests.Domain.ValueObjects;

/// <summary>
/// Generates a tuple of numbers where the first is larger in magnitude than the second.
/// </summary>
[SuppressMessage("Security", "CA5394:Do not use insecure randomness")]
public class LargerMagnitudeFirstSpecimenBuilder : ISpecimenBuilder
{
    public object Create(object request, ISpecimenContext context)
    {
        if (request is not Type type || type != typeof((int First, int Second)))
        {
            return new NoSpecimen();
        }

        // Generate pairs where first has larger magnitude
        var pairs = new[]
        {
            (1000, 10),
            (500, 50),
            (750, 25),
            (900, 30),
            (800, 20)
        };

        var random = new Random();
        return pairs[random.Next(pairs.Length)];
    }
}


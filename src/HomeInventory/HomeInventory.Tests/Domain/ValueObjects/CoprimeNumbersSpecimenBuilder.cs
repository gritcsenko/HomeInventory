using System.Diagnostics.CodeAnalysis;
using AutoFixture.Kernel;

namespace HomeInventory.Tests.Domain.ValueObjects;

/// <summary>
/// Generates a tuple of coprime numbers (GCD = 1).
/// </summary>
[SuppressMessage("Security", "CA5394:Do not use insecure randomness")]
public class CoprimeNumbersSpecimenBuilder : ISpecimenBuilder
{
    public object Create(object request, ISpecimenContext context)
    {
        if (request is not Type type || type != typeof((int Numerator, int Denominator)))
        {
            return new NoSpecimen();
        }

        // Use well-known coprime pairs
        var coprimePairs = new[]
        {
            (7, 11),
            (13, 17),
            (23, 29),
            (31, 37),
            (41, 43),
            (53, 59),
            (61, 67),
            (71, 73),
            (83, 89),
            (97, 101)
        };

        var random = new Random();
        return coprimePairs[random.Next(coprimePairs.Length)];
    }
}


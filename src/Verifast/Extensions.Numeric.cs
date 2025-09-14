using System.Numerics;

namespace Verifast;

public static partial class Extensions {
    /// <summary>
    /// Returns <see langword="true"/> when <paramref name="value"/> is within the inclusive range [<paramref name="lowerBound"/>, <paramref name="upperBound"/>].
    /// </summary>
    /// <typeparam name="T">A numeric type supporting generic math.</typeparam>
    /// <param name="value">The value to evaluate.</param>
    /// <param name="lowerBound">Inclusive lower bound.</param>
    /// <param name="upperBound">Inclusive upper bound.</param>
    /// <returns><see langword="true"/> if <paramref name="value"/> ∈ [<paramref name="lowerBound"/>, <paramref name="upperBound"/>].</returns>
    public static bool IsInRange<T>(this T value, T lowerBound, T upperBound) where T : INumber<T> {
        return value >= lowerBound && value <= upperBound;
    }

    /// <summary>
    /// Returns <see langword="true"/> when <paramref name="value"/> is within the exclusive range (<paramref name="lowerExclusive"/>, <paramref name="upperExclusive"/>).
    /// </summary>
    /// <typeparam name="T">A numeric type supporting generic math.</typeparam>
    /// <param name="value">The value to evaluate.</param>
    /// <param name="lowerExclusive">Exclusive lower bound.</param>
    /// <param name="upperExclusive">Exclusive upper bound.</param>
    /// <returns><see langword="true"/> if <paramref name="value"/> ∈ (<paramref name="lowerExclusive"/>, <paramref name="upperExclusive"/>).</returns>
    public static bool IsInExclusiveRange<T>(this T value, T lowerExclusive, T upperExclusive) where T : INumber<T> {
        return value > lowerExclusive && value < upperExclusive;
    }
}
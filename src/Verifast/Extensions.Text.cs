namespace Verifast;

/// <summary>
/// Text-related validation helpers designed for simple in-line usage.
/// </summary>
public static partial class Extensions {
    /// <summary>
    /// Returns <see langword="true"/> when the string length is within the inclusive range [<paramref name="minInclusive"/>, <paramref name="maxInclusive"/>].
    /// Treats <see langword="null"/> as length 0.
    /// </summary>
    /// <param name="s">The input string (may be <see langword="null"/>).</param>
    /// <param name="minInclusive">Minimum allowed length (inclusive).</param>
    /// <param name="maxInclusive">Maximum allowed length (inclusive).</param>
    /// <returns><see langword="true"/> if the string length is within the specified bounds.</returns>
    public static bool IsLengthBetween(this ReadOnlySpan<char> str, int minInclusive, int maxInclusive) {
        int len = str.Length;
        return len >= minInclusive && len <= maxInclusive;
    }
}
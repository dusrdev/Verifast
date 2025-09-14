using System.Numerics;

namespace Verifast;

public static partial class Extensions {
	public static bool IsInRange<T>(this T value, T lowerBound, T upperBound) where T : INumber<T> {
		return value >= lowerBound && value <= upperBound;
	}
}
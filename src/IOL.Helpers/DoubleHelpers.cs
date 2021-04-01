using System;

namespace IOL.Helpers
{
	public static class DoubleHelpers
	{
		public static string ToStringWithFixedDecimalPoints(this double value) {
			return $"{Math.Truncate(value * 10) / 10:0.0}";
		}
	}
}
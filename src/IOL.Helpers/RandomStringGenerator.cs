using System;
using System.Linq;

namespace IOL.Helpers
{
	public static class RandomString
	{
		private static readonly Random _random = new Random();

		public static string Generate(int length, bool numeric = false) {
			var chars = numeric switch {
				false => "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789",
				true => "0123456789"
			};
			return new string(Enumerable.Repeat(chars, length).Select(s => s[_random.Next(s.Length)]).ToArray());
		}
	}
}
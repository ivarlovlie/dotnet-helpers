using System;
using System.Text;
using System.Text.RegularExpressions;

namespace IOL.Helpers
{
	public static class StringHelpers
	{
		public static bool IsNullOrWhiteSpace(this string value) {
			return string.IsNullOrWhiteSpace(value);
		}

		public static string Slugified(this string input) {
			return Slug.Generate(true, input);
		}

		public static bool HasValue(this string value) {
			return !value.IsNullOrWhiteSpace();
		}

		public static Guid ToGuid(this string value) {
			return Guid.Parse(value);
		}

		public static string Base64Encode(this string text) {
			return Convert.ToBase64String(Encoding.UTF8.GetBytes(text));
		}

		public static string ExtractFileName(this string value) {
			if (value.IsNullOrWhiteSpace()) return default;
			var lastIndex = value.LastIndexOf('.');
			return lastIndex <= 0 ? default : value.Substring(0, lastIndex);
		}

		public static string ExtractExtension(this string value) {
			if (value.IsNullOrWhiteSpace()) return default;
			var lastIndex = value.LastIndexOf('.');
			return lastIndex <= 0 ? default : value.Substring(lastIndex);
		}

		public static string Capitalize(this string input) {
			return input.IsNullOrWhiteSpace()
				? input
				: Regex.Replace(input, @"\b(\w)", m => m.Value.ToUpper(), RegexOptions.None);
		}


		/// <summary>
		/// Check if the given MIME is a JSON MIME.
		/// </summary>
		/// <param name="mime">MIME</param>
		/// <returns>Returns true if MIME type is json.</returns>
		public static bool IsJsonMime(this string mime) {
			var jsonRegex = new Regex("(?i)^(application/json|[^;/ \t]+/[^;/ \t]+[+]json)[ \t]*(;.*)?$");
			return mime != null && (jsonRegex.IsMatch(mime) || mime.Equals("application/json-patch+json"));
		}

		public static string Obfuscate(this string value) {
			var last4Chars = "****";
			if (value.HasValue() && value.Length > 4) {
				last4Chars = value.Substring(value.Length - 4);
			}

			return "****" + last4Chars;
		}
	}
}
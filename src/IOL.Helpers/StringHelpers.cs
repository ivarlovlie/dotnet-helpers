using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace IOL.Helpers;

public static class StringHelpers
{
	/// <summary>
	/// Check wheter or not the given string has a value
	/// </summary>
	/// <param name="input"></param>
	/// <returns>False if the input string is null or only whitespace, otherwise it returns true</returns>
	public static bool IsNullOrWhiteSpace(this string input) {
		return string.IsNullOrWhiteSpace(input);
	}

	/// <summary>
	/// Convert a string to snake_casing
	/// </summary>
	/// <param name="input">The input string to convert</param>
	/// <returns>A snake cased string representation of the input string</returns>
	public static string AsSnakeCasedString(this string input) {
		if (input.IsNullOrWhiteSpace()) return default;
		input = input.ToLower().Trim();
		return input.Replace(" ", "_");
	}

	/// <summary>
	/// Creatas a url safe string (slug) of the input
	/// </summary>
	/// <param name="input">The string to convert into a slug</param>
	/// <returns>A string (slug) representation of the input</returns>
	public static string AsSlug(this string input) {
		return Slug.Generate(true, input);
	}

	/// <summary>
	/// Check wheter or not the given string has a value
	/// </summary>
	/// <param name="input"></param>
	/// <returns>False if the input string is null or only whitespace, otherwise it returns true</returns>
	public static bool HasValue(this string input) {
		return !input.IsNullOrWhiteSpace();
	}

	/// <summary>
	/// Performs a unicorn format on the input.
	/// </summary>
	/// <example>
	///		const string input = "Hello, {name}";
	///		var result = input.UnicornFormat(new Dictionary<string, string> {
	///				{
	///					"name", "World"
	///				}
	///		});
	///		// result -> "Hello, World"
	/// </example>
	/// <param name="input"></param>
	/// <param name="values"></param>
	/// <returns></returns>
	public static string UnicornFormat(this string input, IDictionary<string, string> values) {
		if (string.IsNullOrWhiteSpace(input)) return default;
		if (values.Count == 0) return input;
		return values.Aggregate(input, (current, value1) => current.Replace("{" + value1.Key + "}", value1.Value));
	}

	/// <summary>
	/// Converts a string input to it's Guid representation, if the string does not conform to the guid specification a FormatException will be thrown
	/// </summary>
	/// <param name="value">The string to interpret as a guid</param>
	/// <returns>The input string as a guid</returns>
	public static Guid AsGuid(this string input) {
		return new Guid(input);
	}

	/// <summary>
	/// Converts a string input to it's Guid representation, if the string does not conform to the guid specification, a default input will be returned
	/// </summary>
	/// <param name="value">The string to interpret as a guid</param>
	/// <returns>The string as a guid or the default input for guids</returns>
	public static Guid AsGuidOrDefault(this string input) {
		return !Guid.TryParse(input, out var res) ? default : res;
	}

	/// <summary>
	/// Encodes a string input to it's base 64 equalient
	/// </summary>
	/// <param name="input">The input to encode</param>
	/// <returns>The base 64 encoded string</returns>
	public static string AsBase64EncodedString(this string input) {
		return input.IsNullOrWhiteSpace() ? default : Convert.ToBase64String(Encoding.UTF8.GetBytes(input));
	}

	/// <summary>
	/// Decodes a string input to it's original input
	/// </summary>
	/// <param name="input">The input to decode</param>
	/// <returns>The decoded string input of the base 64 string</returns>
	public static string AsBase64DecodedString(this string input) {
		return input.IsNullOrWhiteSpace() ? default : Encoding.UTF8.GetString(Convert.FromBase64String(input));
	}

	/// <summary>
	/// Capitalize the input
	/// </summary>
	/// <param name="input">The string input to capitalize</param>
	/// <returns></returns>
	public static string Capitalize(this string input, bool onlyFirstChar = false) {
		if (input.IsNullOrWhiteSpace()) return default;
		input = input.Trim();

		if (!onlyFirstChar) {
			return Regex.Replace(input, @"\b(\w)", m => m.Value.ToUpper(), RegexOptions.None);
		}

		input = char.ToUpper(input[0]) + input.Substring(1, input.Length - 1);
		return input;
	}

	/// <summary>
	/// Obfucates the input string
	/// </summary>
	/// <param name="input"></param>
	/// <returns></returns>
	public static string Obfuscate(this string input) {
		if (input.IsNullOrWhiteSpace()) return default;
		var last4Chars = "****";
		if (input.HasValue() && input.Length > 4) {
			last4Chars = input[^4..];
		}

		return "****" + last4Chars;
	}
}

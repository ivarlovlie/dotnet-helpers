using System.Text;

namespace IOL.Helpers;

public static class Slug
{
	public static string Generate(bool toLower, params string[] values) {
		return Create(string.Join("-", values), toLower);
	}

	/// <summary>
	/// Creates a slug.
	/// References:
	/// http://www.unicode.org/reports/tr15/tr15-34.html
	/// https://meta.stackexchange.com/questions/7435/non-us-ascii-characters-dropped-from-full-profile-url/7696#7696
	/// https://stackoverflow.com/questions/25259/how-do-you-include-a-webpage-title-as-part-of-a-webpage-url/25486#25486
	/// https://stackoverflow.com/questions/3769457/how-can-i-remove-accents-on-a-string
	/// </summary>
	/// <param name="value"></param>
	/// <param name="toLower"></param>
	/// <returns>Slugified string</returns>
	public static string Create(string value, bool toLower) {
		if (string.IsNullOrWhiteSpace(value))
			return value;

		var normalised = value.Normalize(NormalizationForm.FormKD);

		const int MAXLEN = 80;
		var len = normalised.Length;
		var prevDash = false;
		var sb = new StringBuilder(len);

		for (var i = 0; i < len; i++) {
			var c = normalised[i];
			switch (c) {
				case >= 'a' and <= 'z':
				case >= '0' and <= '9': {
					if (prevDash) {
						sb.Append('-');
						prevDash = false;
					}

					sb.Append(c);
					break;
				}
				case >= 'A' and <= 'Z': {
					if (prevDash) {
						sb.Append('-');
						prevDash = false;
					}

					// Tricky way to convert to lowercase
					if (toLower)
						sb.Append((char)(c | 32));
					else
						sb.Append(c);
					break;
				}
				case ' ':
				case ',':
				case '.':
				case '/':
				case '\\':
				case '-':
				case '_':
				case '=': {
					if (!prevDash && sb.Length > 0) {
						prevDash = true;
					}

					break;
				}
				default: {
					var swap = ConvertEdgeCases(c, toLower);
					if (swap != null) {
						if (prevDash) {
							sb.Append('-');
							prevDash = false;
						}

						sb.Append(swap);
					}

					break;
				}
			}

			if (sb.Length == MAXLEN)
				break;
		}

		return sb.ToString();
	}

	private static string ConvertEdgeCases(char c, bool toLower) => c switch {
			'ı' => "i",
			'ł' => "l",
			'Ł' => toLower ? "l" : "L",
			'đ' => "d",
			'ß' => "ss",
			'ø' => "o",
			'å' => "aa",
			'æ' => "ae",
			'Þ' => "th",
			_ => null
	};
}

using System.Net.Mail;
using System.Text.RegularExpressions;

namespace IOL.Helpers;

public static class Validators
{
	private static readonly Regex _norwegianPhoneNumber = new(@"^(0047|\+47|47)?[2-9]\d{7}$");

	public static bool IsValidEmailAddress(this string value) {
		return MailAddress.TryCreate(value, out _);
	}

	public static bool IsValidNorwegianPhoneNumber(this string value) {
		return _norwegianPhoneNumber.IsMatch(value);
	}
}

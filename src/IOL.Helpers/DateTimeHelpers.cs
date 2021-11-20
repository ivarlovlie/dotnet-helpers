using System;

namespace IOL.Helpers;

public static class DateTimeHelpers
{
	public static DateTime ToTimeZoneId(this DateTime value, string timeZoneId) {
		try {
			var cstZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
			return TimeZoneInfo.ConvertTimeFromUtc(value, cstZone);
		} catch (TimeZoneNotFoundException) {
			Console.WriteLine("The registry does not define the " + timeZoneId + " zone.");
			return default;
		} catch (InvalidTimeZoneException) {
			Console.WriteLine("Registry data on the " + timeZoneId + " zone has been corrupted.");
			return default;
		}
	}

	public static DateTime ToOsloTimeZone(this DateTime value) => ToTimeZoneId(value, "Europe/Oslo");

	public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek) {
		var diff = (7 + (dt.DayOfWeek - startOfWeek)) % 7;
		return dt.AddDays(-1 * diff).Date;
	}
}

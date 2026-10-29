using System;
using System.Globalization;

namespace PolicyTrackerWebClient.Infrastructure
{
    public static class ExtensionMethods
    {
        private static readonly CultureInfo India = CultureInfo.GetCultureInfo("en-IN");

        public static bool IsNotNullOrEmpty(this string inputtedString)
        {
            return !string.IsNullOrWhiteSpace(inputtedString);
        }

        public static bool IsNullOrEmpty(this string inputtedString)
        {
            return string.IsNullOrWhiteSpace(inputtedString);
        }

        public static string StrToUpper(this string inputtedString)
        {
            return inputtedString.IsNotNullOrEmpty() ? inputtedString.Trim().ToUpper() : inputtedString;
        }

        public static string StrToLower(this string inputtedString)
        {
            return inputtedString.IsNotNullOrEmpty() ? inputtedString.Trim().ToLower() : inputtedString;
        }

        public static string ToEnumString(this Enum enumValue)
        {
            return enumValue == null ? string.Empty : Convert.ToString(enumValue).Replace("_", " ");
        }

        /// <summary>₹ with Indian digit grouping: 2,42,919</summary>
        public static string ToRupees(this decimal? value)
        {
            return value.HasValue ? value.Value.ToRupees() : "-";
        }

        public static string ToRupees(this decimal value)
        {
            return "₹" + Math.Round(value, 0, MidpointRounding.AwayFromZero).ToString("#,##0", India);
        }

        public static string ToDisplayDate(this DateTime? value)
        {
            return value.HasValue ? value.Value.ToString("dd-MMM-yy", CultureInfo.InvariantCulture) : string.Empty;
        }

        public static string ToMonthText(this DateTime? value)
        {
            return value.HasValue ? value.Value.ToString("MMM-yyyy", CultureInfo.InvariantCulture) : string.Empty;
        }

        public static string ToMonthText(this DateTime value)
        {
            return value.ToString("MMM-yyyy", CultureInfo.InvariantCulture);
        }

        /// <summary>Parses "2026-10" / "2026-10-01" from month / date inputs.</summary>
        public static DateTime? ToMonth(this string value)
        {
            if (value.IsNullOrEmpty()) return null;
            DateTime d;
            if (DateTime.TryParseExact(value.Trim(), new[] { "yyyy-MM", "yyyy-MM-dd", "MMM-yyyy" }, CultureInfo.InvariantCulture, DateTimeStyles.None, out d))
                return new DateTime(d.Year, d.Month, 1);
            return null;
        }
    }
}

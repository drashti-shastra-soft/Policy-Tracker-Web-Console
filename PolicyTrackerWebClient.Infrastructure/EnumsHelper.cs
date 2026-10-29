using System;
using System.Collections.Generic;
using System.Linq;

namespace PolicyTrackerWebClient.Infrastructure
{
    public class EnumsHelper
    {
        /// <summary>Quote_Sent -> "Quote Sent"</summary>
        public static string GetEnumString(Enum value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            return Convert.ToString(value).Replace("_", " ");
        }

        public static int GetEnumIndex(Enums.Role role)
        {
            return (int)role;
        }

        public static List<string> GetTexts<TEnum>() where TEnum : struct
        {
            return Enum.GetValues(typeof(TEnum)).Cast<Enum>().Select(GetEnumString).ToList();
        }

        public static bool Is(string text, Enum value)
        {
            return string.Equals((text ?? string.Empty).Trim(), GetEnumString(value), StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>Bootstrap badge class for a status text.</summary>
        public static string BadgeClass(string status)
        {
            switch ((status ?? string.Empty).Trim())
            {
                case "Found":
                case "Renewed":
                case "Matched Policy No":
                    return "badge-success";
                case "Missing":
                case "Lost":
                case "Other POS Code":
                case "Unmatched":
                    return "badge-danger";
                case "Overdue":
                case "Quote Sent":
                case "Matched Name Date":
                    return "badge-warning";
                case "Follow Up":
                case "Awaiting Statement":
                case "Manual":
                    return "badge-info";
                default:
                    return "badge-secondary";
            }
        }
    }
}

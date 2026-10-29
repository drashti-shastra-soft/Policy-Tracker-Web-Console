using System;
using System.Web;
using System.Web.Mvc;

namespace PolicyTrackerWebClient
{
    public static class HtmlExtensions
    {
        /// <summary>Writes a value as a quoted, escaped JavaScript string literal.</summary>
        public static IHtmlString Js(this HtmlHelper html, object value)
        {
            return new HtmlString(HttpUtility.JavaScriptStringEncode(Convert.ToString(value) ?? string.Empty, true));
        }

        /// <summary>Badge for days left to expiry.</summary>
        public static IHtmlString DaysLeft(this HtmlHelper html, int? days)
        {
            if (!days.HasValue) return new HtmlString(string.Empty);
            var d = days.Value;
            string cls, text;
            if (d < 0) { cls = "badge-dark"; text = (-d) + "d overdue"; }
            else if (d == 0) { cls = "badge-danger"; text = "Today"; }
            else if (d <= 7) { cls = "badge-danger"; text = d + (d == 1 ? " day" : " days"); }
            else if (d <= 15) { cls = "badge-warning"; text = d + " days"; }
            else { cls = "badge-info"; text = d + " days"; }
            return new HtmlString("<span class=\"badge days-left " + cls + "\">" + HttpUtility.HtmlEncode(text) + "</span>");
        }

        public static IHtmlString StatusBadge(this HtmlHelper html, string status)
        {
            if (string.IsNullOrWhiteSpace(status)) return new HtmlString(string.Empty);
            return new HtmlString("<span class=\"badge " + Infrastructure.EnumsHelper.BadgeClass(status) + "\">" + HttpUtility.HtmlEncode(status) + "</span>");
        }

        /// <summary>Call and WhatsApp links for an Indian mobile number.</summary>
        public static IHtmlString ContactLinks(this HtmlHelper html, string mobile)
        {
            if (string.IsNullOrWhiteSpace(mobile)) return new HtmlString(string.Empty);
            var digits = new string(Array.FindAll(mobile.ToCharArray(), char.IsDigit));
            if (digits.Length == 10) digits = "91" + digits;
            var enc = HttpUtility.HtmlEncode(mobile.Trim());
            return new HtmlString(
                "<span class=\"contact-links nowrap\">" +
                "<a href=\"tel:+" + digits + "\" title=\"Call\"><i class=\"fas fa-phone-alt\"></i></a>" +
                "<a href=\"https://wa.me/" + digits + "\" target=\"_blank\" rel=\"noopener\" title=\"WhatsApp\"><i class=\"fab fa-whatsapp\"></i></a>" +
                enc + "</span>");
        }
    }
}

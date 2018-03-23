using System;
using System.Text.RegularExpressions;
using System.Linq;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Html;

namespace MyTeam
{
    public static class StringExtensions
    {
        
        public static string Pluralize(this string str, int count = 0)
        {
            if (count == 1) return str;

            if (Array.IndexOf(Exceptions, str.ToLower()) > -1)
                return str;
            if (str.EndsWith("e", StringComparison.CurrentCultureIgnoreCase))
                return $"{str}r";
            return $"{str}er";
        }

        private static string[] Exceptions =
         {
            "diverse"
        };

        public static string Truncate(this string str, int length = 450, bool excludeDots = false)
        {
            var dots = excludeDots ? "" : "...";
            if (str.Length > length) return $"{str?.Substring(0, length)}{dots}";
            return str;
        }

        public static string StripHtmlTags(this string str)
        {
            return Regex.Replace(str, "<.*?>", String.Empty);
        }

        public static DateTime? AsDate(this string str)
        {
            try
            {
                DateTime date;
                var success = DateTime.TryParse(str, out date);

                if (!success)
                {
                    var dateArray = str.Split('.');
                    date = new DateTime(int.Parse(dateArray[2]), int.Parse(dateArray[1]), int.Parse(dateArray[0]));
                }
                return date;

            }
            catch (Exception)
            {
                return null;
            }
           
        }

        public static TimeSpan? AsTime(this string str)
        {
         try
            {
                var dateArray = str.Split(':');
                var hour = int.Parse(dateArray[0]);
                var minute = dateArray.Length > 1 ? int.Parse(dateArray[1]) : 0;
                return new TimeSpan(hour, minute, 0);
            }
            catch (Exception)
            {
                return null;
            }
           
        }

        public static HtmlString IncludeLinebreaks(this string str)
        {
            var result = string.Join(
                "<br/>",
                str.Split(new[] { Environment.NewLine }, StringSplitOptions.None).Select(x => HtmlEncoder.Default.Encode(x))
            );
            return new HtmlString(result);
        }

    }
}
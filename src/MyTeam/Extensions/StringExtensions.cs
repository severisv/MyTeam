using System;
using System.Collections;

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

        public static string Truncate(this string str, int length = 300)
        {
            if(str.Length > 300) return $"{str?.Substring(0, length)}...";
            return str;
        }


        public static DateTime? AsDate(this string str)
        {
            try
            {
                return DateTime.Parse(str);
            }
            catch(Exception)
            { }

            try
            {
                var dateArray = str.Split('.');
                return new DateTime(int.Parse(dateArray[2]), int.Parse(dateArray[1]), int.Parse(dateArray[0]));
            }
            catch (Exception)
            {
                return null;
            }
           
        }
    }
}
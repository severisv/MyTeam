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
    }
}
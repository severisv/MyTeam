using System;

namespace MyTeam
{
    public static class DateTimeExtensions
    {
        public static string ToNo(this DateTime datetime)
        {
            return datetime.ToString("ddd d MMMM");
        }
    }
}
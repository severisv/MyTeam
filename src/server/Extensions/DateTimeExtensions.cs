using System;

namespace MyTeam
{
    public static class DateTimeExtensions
    {
        public static string ToNo(this DateTime datetime)
        {
            return datetime.ToString("ddd d MMMM");
        }

        public static string ToNoFull(this DateTime? datetime)
        {
            if (datetime == null) return string.Empty;
            return datetime.Value.ToString("dd.MM.yyyy");
        }

        public static string ToNoFull(this DateTime datetime)
        {
            return datetime.ToString("dd.MM.yyyy");
        }

        public static string ToNoShort(this DateTime datetime)
        {
            return datetime.ToString("dd.MM");
        }

        public static string ToNo(this TimeSpan timespan)
        {
            return timespan.ToString(@"hh\:mm");
        }

        public static string ToNoTime(this DateTime dateTime)
        {
            return dateTime.ToString(@"HH\:mm");
        }
    }
}
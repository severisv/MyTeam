using System;

namespace MyTeam.Models.Shared
{
    public static class EventExtensions
    {
        public static bool SignupHasClosed(this IEvent ev) => ev.HasPassed();
        public static bool HasPassed(this IEvent ev) => ev.DateTime < DateTime.Now;

    }
}
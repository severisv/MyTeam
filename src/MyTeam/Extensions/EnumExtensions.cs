using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace MyTeam
{
    public static class Extensions
    {
        public static string DisplayName(this Enum enumValue)
        {
            return enumValue.GetType()
                            .GetMember(enumValue.ToString())
                            .First().GetCustomAttribute<DisplayAttribute>()?.GetName() ?? enumValue.ToString();
        }
    }
}
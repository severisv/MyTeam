using System;

namespace MyTeam.Models.Shared
{
    public static class MemberExtensions
    {

        public static string Fullname(this IMember m) => $"{m.FirstName} {m.MiddleName} {m.LastName}";
        public static string Name(this IMember m) => $"{m.FirstName} {m.LastName}";


    }
}
using System;

namespace MyTeam.Models.Shared
{
    public static class MemberExtensions
    {

        public static string Name(this IMember m) => $"{m.FirstName} {m.LastName}";


    }
}
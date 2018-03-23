using System;
using MyTeam.Models.Enums;

namespace MyTeam.Models.Shared
{
    public interface IMember
    {
        string FirstName { get; }
        string MiddleName { get; }
        string LastName { get; }
    }
}

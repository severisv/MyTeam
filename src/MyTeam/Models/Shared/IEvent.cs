using System;
using MyTeam.Models.Enums;

namespace MyTeam.Models.Shared
{
    public interface IEvent
    {
        DateTime DateTime { get; }
    }
}

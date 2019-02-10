using System;
using MyTeam.Extensions;
using MyTeam.Models.Enums;
using MyTeam;

namespace MyTeam.ViewModels.Game
{
    public class GameEventViewModel
    {
        [RequiredNO]
        public GameEventType Type { get; set; }

        public Guid? PlayerId { get; set; }
        [RequiredNO]
        public Guid GameId { get; set; }
        public Guid? AssistedById { get; set; }
    }
}
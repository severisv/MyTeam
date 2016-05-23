using System;
using MyTeam.Extensions;
using MyTeam.Models.Enums;
using MyTeam.Validation.Attributes;

namespace MyTeam.ViewModels.Game
{
    public class GameEventViewModel
    {
        [RequiredNO]
        public GameEventType Type { get; set; }

        public string TypeName => Type.DisplayName();
        public string TypeValueName => Type.ToString();
        public Guid? PlayerId { get; set; }
        public string PlayerName { get; set; }
        [RequiredNO]
        public Guid GameId { get; set; }
        public Guid? AssistedById { get; set; }
        public string AssistedByName { get; set; }
        public Guid Id { get; set; }


     
      
    }
}
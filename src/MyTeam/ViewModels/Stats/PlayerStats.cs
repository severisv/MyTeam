using System;
using System.Collections.Generic;
using System.Linq;
using MyTeam.Models.Enums;
using MyTeam.Models.Shared;

namespace MyTeam.ViewModels.Stats
{
    public class PlayerStats : IMember
    {

        private readonly IEnumerable<Models.Domain.GameEvent> _gameEvents;
        private readonly IEnumerable<Guid> _eventAttendances;

        public PlayerStats(Guid id, string facebookId, string firstName, string middleName, string lastName, string imageFull, string urlName, IEnumerable<Models.Domain.GameEvent> gameEvents, IEnumerable<Guid> attendances)
        {
            Id = id;
            FacebookId = facebookId;
            FirstName = firstName;
            MiddleName = middleName;
            LastName = lastName;
            Image = imageFull;
            UrlName = urlName;
            _gameEvents = gameEvents;
            _eventAttendances = attendances;
        }

        public Guid Id { get; set; }
        public string FacebookId { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string UrlName { get; set; }
        public string Image { get; set; }
        public int Games => _eventAttendances.Count(a => a == Id);
        public int Goals => _gameEvents.Count(ge => ge.Type == GameEventType.Goal && ge.PlayerId == Id);
        public int Assists => _gameEvents.Count(ge => ge.Type == GameEventType.Goal && ge.AssistedById == Id);
        public int YellowCards => _gameEvents.Count(ge => ge.Type == GameEventType.YellowCard && ge.PlayerId == Id);
        public int RedCards => _gameEvents.Count(ge => ge.Type == GameEventType.RedCard && ge.PlayerId == Id);

                     
      
    }
}
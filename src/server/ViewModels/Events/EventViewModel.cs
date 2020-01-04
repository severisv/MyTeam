using System;
using System.Collections.Generic;
using System.Linq;
using MyTeam.Models.Domain;
using MyTeam.Models.Enums;
using MyTeam.Models.Shared;

namespace MyTeam.ViewModels.Events
{
    public class EventViewModel : IEvent
    {
        public Guid Id { get; }
        public Guid ClubId { get; }
        public EventType Type { get; }
        public GameType? GameType { get; }
        public DateTime DateTime { get; }
        public string Location { get; }
        public string Headline { get; }
        public string Description { get; }
        public bool Voluntary { get; }
        public bool IsHomeTeam { get; }
        public bool? GamePlanIsPublished { get; }
        public string Opponent { get; }
        public IEnumerable<Guid> TeamIds { get; }


        public EventViewModel(Guid clubId, IEnumerable<Guid> teamIds, Guid eventId, EventType type, GameType? gameType, DateTime dateTime, string location,
            string headline, string description, string opponent, bool voluntary, bool isPublished, bool isHomeTeam, bool? gamePlanIsPublished)
        {
            Id = eventId;
            ClubId = clubId;
            DateTime = dateTime;
            Location = location;
            Headline = headline;
            Description = description;
            Type = type;
            GameType = gameType;
            Opponent = opponent;
            Voluntary = voluntary;
            TeamIds = teamIds;
            IsPublished = isPublished;
            IsHomeTeam = isHomeTeam;
            GamePlanIsPublished = gamePlanIsPublished;
        }

        public EventViewModel(Event e) : this(e.ClubId, e.EventTeams.Select(t => t.TeamId), e.Id, e.Type.FromInt(), e.GameTypeValue, e.DateTime, e.Location, e.Headline, e.Description, e.Opponent, e.Voluntary, e.IsPublished, e.IsHomeTeam, e.GamePlanIsPublished)
        {

        }

        public bool IsPublished { get; }
     

        public CurrentTeam Team(IEnumerable<CurrentTeam> teams)
        {
            return teams.First(t => t.Id == TeamIds.First());
        }


    }


}
using System;
using System.Collections.Generic;
using System.Linq;
using MyTeam.Models;
using MyTeam.Models.Domain;
using MyTeam.Models.Enums;
using MyTeam.ViewModels.Game;

namespace MyTeam.Services.Domain
{
    class GameService : IGameService
    {

        private readonly ApplicationDbContext _dbContext;

        public GameService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void SelectPlayer(Guid eventId, Guid playerId, bool isSelected)
        {
            var attendance = _dbContext.EventAttendances.FirstOrDefault(a => a.EventId == eventId && a.MemberId == playerId);
            if (attendance != null)
            {
                attendance.IsSelected = isSelected;
            }
            else
            {
                attendance = new EventAttendance
                {
                    Id = Guid.NewGuid(),
                    EventId = eventId,
                    IsSelected = isSelected,
                    IsAttending = false,
                    MemberId = playerId
                };
                _dbContext.EventAttendances.Add(attendance);
            }
            _dbContext.SaveChanges();
        }
        

        public RegisterSquadEventViewModel GetRegisterSquadEventViewModel(Guid eventId)
        {
            return _dbContext.Events.Where(e => e.Id == eventId)
                .Select(e => new RegisterSquadEventViewModel
                {
                    DateTime = e.DateTime,
                    Attendees = e.Attendees.Select(a => new RegisterSquadAttendeeViewModel
                    {
                        MemberId = a.MemberId,
                        IsSelected = a.IsSelected,
                        SignupMessage = a.SignupMessage,
                        IsAttending = a.IsAttending
                    }).ToList(),
                    Id = e.Id,
                    Location = e.Location,
                    Description = e.Description,
                    Type = e.Type,
                    IsPublished = e.IsPublished
                }).Single();
        }

        public void PublishSquad(Guid eventId)
        {
            var ev = _dbContext.Events.Single(e => e.Id == eventId);
            ev.IsPublished = true;
            _dbContext.SaveChanges();
        }

        public IEnumerable<GameViewModel> GetGames(Guid teamId, int year)
        {
            var startDate = new DateTime(year, 1,1);
            var endDate = new DateTime(year, 12,31);

            var games = _dbContext.Events
                .Where(e => e.Type == EventType.Kamp)
                .Where(e => e.EventTeams.Count(et => et.TeamId == teamId) > 0)
                .Where(e => e.DateTime.Date >= startDate && e.DateTime.Date <= endDate)
                .Select(e => new GameViewModel
                {
                    DateTime = e.DateTime,
                    Opponent = e.Opponent,
                    Teams = e.EventTeams.Select(et => et.Team.Name),
                    Id = e.Id,
                    HomeScore = e.HomeScore,
                    AwayScore = e.AwayScore,
                    IsHomeTeam = e.IsHomeTeam,
                    Location = e.Location
                }).ToList();

            return games;

        }

        public IEnumerable<SeasonViewModel> GetSeasons(Guid teamId)
        {

            var years = _dbContext.Events
                .Where(e => e.EventTeams.Count(et => et.TeamId == teamId) > 0)
                .Where(e => e.Type == EventType.Kamp).Select(e => e.DateTime.Year).ToList().Distinct();
            
            return years.Select(y => new SeasonViewModel
            {
                TeamId = teamId,
                Year = y
            }).OrderByDescending(s => s.Year);
        }
    }
}
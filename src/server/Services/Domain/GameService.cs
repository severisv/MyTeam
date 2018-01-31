using System;
using System.Collections.Generic;
using System.Linq;
using MyTeam.Models;
using MyTeam.Models.Domain;
using MyTeam.Models.Enums;
using MyTeam.Services.Application;
using MyTeam.ViewModels.Game;

namespace MyTeam.Services.Domain
{
    class GameService : IGameService
    {

        private readonly ApplicationDbContext _dbContext;
        private readonly ICacheHelper _cacheHelper;

        public GameService(ApplicationDbContext dbContext, ICacheHelper cacheHelper)
        {
            _dbContext = dbContext;
            _cacheHelper = cacheHelper;
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
                    IsPublished = e.IsPublished,
                    TeamIds = e.EventTeams.Select(et => et.TeamId).ToList()
                }).Single();
        }

        public void PublishSquad(Guid eventId)
        {
            var ev = _dbContext.Events.Single(e => e.Id == eventId);
            ev.IsPublished = true;
            _dbContext.SaveChanges();
            _cacheHelper.ClearNotificationCache(ev.ClubId);
        }

        public void PublishGamePlan(Guid eventId)
        {
            var ev = _dbContext.Games.Single(e => e.Id == eventId);
            ev.GamePlanIsPublished = true;
            _dbContext.SaveChanges();
        }

        public string GetGamePlan(Guid eventId)
            =>
            _dbContext.Games.Where(e => e.Id == eventId).Select(g => g.GamePlan).Single();


        public void SaveGamePlan(Guid eventId, string gamePlan)
        {
            var ev = _dbContext.Games.Single(e => e.Id == eventId);
            ev.GamePlan = gamePlan;
            _dbContext.SaveChanges();
        }

        public IEnumerable<GameViewModel> GetGames(Guid teamId, int year, string teamName)
        {
            var startDate = new DateTime(year, 1, 1);
            var endDate = new DateTime(year, 12, 31);

            var games = _dbContext.Games
                .Where(e => e.TeamId == teamId)
                .Where(e => e.DateTime.Date >= startDate && e.DateTime.Date <= endDate)
                .Select(e => new GameViewModel
                {
                    DateTime = e.DateTime,
                    Opponent = e.Opponent,
                    Teams = new List<string> { teamName },
                    Id = e.Id,
                    GamePlanIsPublished = e.GamePlanIsPublished,
                    HomeScore = e.HomeScore,
                    AwayScore = e.AwayScore,
                    IsHomeTeam = e.IsHomeTeam,
                    Location = e.Location,
                    GameType = e.GameType
                }).ToList();

            return games.OrderBy(g => g.DateTime);

        }

        public IEnumerable<SeasonViewModel> GetSeasons(Guid teamId)
        {

            var years = _dbContext.Games
                .Where(e => e.TeamId == teamId)
                .Select(e => e.DateTime.Year).ToList().Distinct();

            return years.Select(y => new SeasonViewModel
            {
                Year = y
            }).OrderByDescending(s => s.Year);
        }

        public GameViewModel GetGame(Guid gameId)
            =>
              _dbContext.Games
              .Where(e => e.Id == gameId)
              .Select(e => new GameViewModel
              {
                  DateTime = e.DateTime,
                  Opponent = e.Opponent,
                  Teams = e.EventTeams.Select(et => et.Team.Name),
                  Id = e.Id,
                  GamePlanIsPublished = e.GamePlanIsPublished,
                  HomeScore = e.HomeScore,
                  AwayScore = e.AwayScore,
                  IsHomeTeam = e.IsHomeTeam,
                  Location = e.Location,
                  GameType = e.GameType
              }).ToList().SingleOrDefault();


        public IEnumerable<PlayerViewModel> GetSquad(Guid gameId) =>
             _dbContext.EventAttendances.Where(e => e.EventId == gameId && e.IsSelected)
                    .Select(g => new PlayerViewModel
                    {
                        Id = g.Member.Id,
                        FirstName = g.Member.FirstName,
                        MiddleName = g.Member.MiddleName,
                        LastName = g.Member.LastName,
                        FullName = g.Member.Fullname,
                        UrlName = g.Member.UrlName
                    })
                    .ToList()
                    .OrderBy(p => p.FullName);

        public void AddGames(List<ParsedGame> games, Guid clubId)
        {
            var gameEntities = games.Select(game => new Game
            {
                Id = game.Id,
                DateTime = game.DateTime,
                IsHomeTeam = game.IsHomeTeam,
                AwayScore = game.AwayScore,
                HomeScore = game.HomeScore,
                Location = game.Location,
                Opponent = game.Opponent,
                TeamId = game.TeamId,
                GameType = game.GameType,
                Type = EventType.Kamp,
                ClubId = clubId,
                EventTeams = new List<EventTeam>
                {
                    new EventTeam
                {
                    Id = Guid.NewGuid(),
                    TeamId = game.TeamId,
                    EventId = game.Id
                }}
            }).ToList();
            _dbContext.Games.AddRange(gameEntities);
            _dbContext.SaveChanges();
        }
    }
}
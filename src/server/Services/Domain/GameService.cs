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
                    Type = e.Type.FromInt(),
                    IsPublished = e.IsPublished,
                    TeamIds = e.EventTeams.Select(et => et.TeamId).ToList()
                }).Single();
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
                  GameType = e.GameTypeValue
              }).ToList().SingleOrDefault();


    }
}
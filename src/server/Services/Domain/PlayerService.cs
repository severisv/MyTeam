using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using MyTeam.Models;
using MyTeam.Models.Domain;
using MyTeam.Models.Dto;
using MyTeam.Models.Enums;
using MyTeam.Models.Structs;
using MyTeam.Resources;
using MyTeam.Services.Application;
using MyTeam.ViewModels.Game;
using MyTeam.ViewModels.Player;

namespace MyTeam.Services.Domain
{

    class PlayerService : IPlayerService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ICacheHelper _cacheHelper;

        public PlayerService(ApplicationDbContext dbContext, ICacheHelper cacheHelper)
        {
            _cacheHelper = cacheHelper;
            _dbContext = dbContext;
        }

      
        public void EditPlayer(EditPlayerViewModel model, Guid clubId)
        {
            var player = _dbContext.Members.Single(p => p.Id == model.PlayerId);
            player.FirstName = model.FirstName;
            player.MiddleName = model.MiddleName;
            player.LastName = model.LastName;
            player.Phone = model.Phone;
            player.StartDate = model.StartDate.AsDate();
            player.BirthDate = model.BirthDate.AsDate();
            player.PositionsString = model.PositionsString;
            player.ProfileIsConfirmed = true;
            player.ImageFull = model.ImageFull;
            _dbContext.SaveChanges();
            _cacheHelper.ClearCache(clubId, player.UserName);
        }

        public void AddEmailToPlayer(string facebookId, string email)
        {
            if (string.IsNullOrWhiteSpace(facebookId)) return;
            var players = _dbContext.Members.Where(p => p.FacebookId == facebookId).ToList();
            foreach (var player in players)
            {
                if (string.IsNullOrWhiteSpace(player.UserName))
                {
                    player.UserName = email;
                }
            }
            _dbContext.SaveChanges();
        }

      
        public ShowPlayerViewModel GetSingle(Guid clubId, string name)
        {
            var query = _dbContext.Members.Where(p => p.UrlName == name.ToLower() && p.ClubId == clubId);
            return GetPlayer(query);
        }

        public ShowPlayerViewModel GetSingle(Guid playerId)
        {
            var query = _dbContext.Members.Where(p => p.Id == playerId);
            return GetPlayer(query);
        }
        private ShowPlayerViewModel GetPlayer(IQueryable<Member> query)
        {
            var player = query.Select(p => new ShowPlayerViewModel
            {
                Id = p.Id,
                BirthDate = p.BirthDate,
                FirstName = p.FirstName,
                MiddleName = p.MiddleName,
                LastName = p.LastName,
                UserName = p.UserName,
                StartDate = p.StartDate,
                Phone = p.Phone,
                ImageFull = p.ImageFull,
                PositionsString = p.PositionsString,
                FacebookId = p.FacebookId,
                Status = (PlayerStatus)p.Status,
                UrlName = p.UrlName
            }).SingleOrDefault();

            if (player == null) return null;

            var now = DateTime.Now;

            var practiceCount =
                _dbContext.EventAttendances.Count(
                    e => e.MemberId == player.Id && e.DidAttend && e.Event.DateTime.Year == now.Year);

            player.PracticeCount = practiceCount;

            return player;
        }



        public IEnumerable<ListPlayerViewModel> GetPlayers(PlayerStatus status, Guid clubId)
        {
            return
                _dbContext.Members.Where(p => p.Status == (int)status && p.ClubId == clubId)
                    .OrderBy(p => p.FirstName)
                    .Select(p => new ListPlayerViewModel
                    {
                        Id = p.Id,
                        FirstName = p.FirstName,
                        MiddleName = p.MiddleName,
                        LastName = p.LastName,
                        FacebookId = p.FacebookId,
                        Image = p.ImageFull,
                        UrlName = p.UrlName
                    }).ToList();
        }

        public IEnumerable<PlayerStatsViewModel> GetStats(Guid playerId, IEnumerable<Guid> teamIds)
        {
            var events = _dbContext.GameEvents
                .Include(ge => ge.Game)
                .Where(e => (e.PlayerId == playerId || e.AssistedById == playerId) && e.Game.GameType != (int)GameType.Treningskamp)
                .ToList();

            var now = DateTime.Now;
            var games = _dbContext.Games.Where(
                          g => teamIds.Contains(g.TeamId.Value) &&
                          g.GameType != (int)GameType.Treningskamp &&
                          g.DateTime < now
                          && g.Attendees.Any(a => a.MemberId == playerId && a.IsSelected)
                          )
                .Select(g => new GameAttendanceViewModel
                {
                    TeamId = g.TeamId.Value,
                    DateTime = g.DateTime
                }).ToList();



            var years = games
                .Select(g => new Key { TeamId = g.TeamId, Year = g.DateTime.Year })
                .Distinct()
                .Concat(teamIds.Select(tid => new Key { TeamId = tid, Year = 0 }));


            var byTeamAndYear = events
                            .GroupBy(e => new { TeamId = e.Game.TeamId, Year = e.Game.DateTime.Year })
                            .Concat(
                                    events.GroupBy(e => new { TeamId = e.Game.TeamId, Year = 0 }))

                                    .ToList();



            var grouped = years.Select(key =>
                new EventList
                {
                    Key = key,
                    Items = byTeamAndYear.Where(k => k.Key.TeamId == key.TeamId && k.Key.Year == key.Year).SelectMany(g => g.ToList()).ToList()
                }
            );

            return grouped.Select(group =>
                    new PlayerStatsViewModel(
                        playerId,
                        group.Key.TeamId,
                        group.Items.Select(ge => new GameEventViewModel
                        {
                            AssistedById = ge.AssistedById,
                            PlayerId = ge.PlayerId,
                            GameId = ge.Game.Id,
                            Type = ge.Type
                        }),
                            games.Count(g => g.TeamId == group.Key.TeamId && (g.DateTime.Year == group.Key.Year || group.Key.Year == 0)),
                            group.Key.Year
                        ))
                        .ToList()
                        .OrderByDescending(p => p.Year)
                        .ThenByDescending(p => p.TeamId);





        }
    }

    struct Key
    {
        public Guid TeamId { get; set; }
        public int Year { get; set; }
    }

    struct EventList
    {
        public Key Key { get; set; }
        public List<GameEvent> Items { get; set; }
    }
}
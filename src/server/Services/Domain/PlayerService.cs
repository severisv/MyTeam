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

        public JsonResponseMessage Add(string clubId, string facebookId, string firstName, string middleName, string lastName, string emailAddress)
        {
            var existingPlayer = string.IsNullOrWhiteSpace(facebookId) ?
                _dbContext.Players.FirstOrDefault(p => p.UserName == emailAddress) :
                _dbContext.Players.FirstOrDefault(p => p.FacebookId == facebookId);


            if (!string.IsNullOrWhiteSpace(facebookId) && string.IsNullOrWhiteSpace(emailAddress))
            {
                var correspondingUserLogin = _dbContext.UserLogins.FirstOrDefault(u => u.ProviderKey == facebookId);
                if (correspondingUserLogin != null)
                {
                    emailAddress = _dbContext.Users.Single(u => u.Id == correspondingUserLogin.UserId).Email;
                }
            }


            if (existingPlayer == null)
            {
                var club = _dbContext.Clubs.Single(c => c.ClubIdentifier == clubId);

                var player = new Player
                {
                    FacebookId = facebookId,
                    FirstName = firstName,
                    LastName = lastName,
                    UserName = emailAddress,
                    ClubId = club.Id,
                    MiddleName = middleName,
                    UrlName = GetUrlName(club.Id, firstName, middleName, lastName)
                };

                _dbContext.Players.Add(player);
                _dbContext.SaveChanges();

                var message = string.IsNullOrWhiteSpace(facebookId)
                    ? $"{Res.Player} {Res.Added.ToLower()}"
                    : "facebookAdd";

                return JsonResponse.Success(message);
            }
            return JsonResponse.ValidationFailed($"{Res.Player} {Res.IsAlready.ToLower()} {Res.Added.ToLower()}");
        }

        private string GetUrlName(Guid clubId, string firstName, string middleName, string lastName)
        {
            var middle = !string.IsNullOrWhiteSpace(middleName) ? "-" + middleName.ToLower() : "";
            var urlName = $"{firstName.ToLower()}{middle}-{lastName.ToLower()}";

            var result = urlName.Replace(" ", "-").ToLower();
            result = result.Replace("�", "O");
            result = result.Replace("�", "o");
            result = result.Replace("�", "ae");
            result = result.Replace("�", "Ae");
            result = result.Replace("�", "Aa");
            result = result.Replace("�", "aa");
            result = result.Replace("�", "e");
            result = result.Replace("�", "e");
            Regex rgx = new Regex("[^a-zA-Z0-9 -]");
            result = rgx.Replace(result, "");
            while (_dbContext.Members.Any(m => m.ClubId == clubId && m.UrlName == result))
            {
                result += "-1";
            }
            return result;
        }

        public void SetPlayerStatus(Guid id, PlayerStatus status, Guid clubId)
        {
            var player = _dbContext.Players.Single(p => p.Id == id);
            player.Status = status;
            _dbContext.SaveChanges();
            _cacheHelper.ClearCache(clubId, player.UserName);
        }

        public void TogglePlayerRole(Guid id, string role, Guid clubId)
        {
            var player = _dbContext.Players.Single(p => p.Id == id);
            var roles = player.Roles.ToList();
            if (roles.Any(r => r == role))
            {
                roles.Remove(role);
            }
            else
            {
                roles.Add(role);
            }
            player.RolesString = string.Join(",", roles);
            _dbContext.SaveChanges();
            _cacheHelper.ClearCache(clubId, player.UserName);
        }

        public void EditPlayer(EditPlayerViewModel model, Guid clubId)
        {
            var player = _dbContext.Players.Single(p => p.Id == model.PlayerId);
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
            var players = _dbContext.Players.Where(p => p.FacebookId == facebookId).ToList();
            foreach (var player in players)
            {
                if (string.IsNullOrWhiteSpace(player.UserName))
                {
                    player.UserName = email;
                }
            }
            _dbContext.SaveChanges();
        }

        public IEnumerable<SimplePlayerDto> GetDto(Guid clubId, PlayerStatus? status = null, bool includeCoaches = false)
        {
            var query =
                _dbContext.Players.Where(p => p.ClubId == clubId)
                    .Where(p => p.Status != PlayerStatus.Sluttet);

            if (includeCoaches != true) query = query.Where(p => p.Status != PlayerStatus.Trener);

            if (status != null) query = query.Where(p => p.Status == status);

            var players = query
                .Select(p => new SimplePlayerDto
                {
                    Id = p.Id,
                    FacebookId = p.FacebookId,
                    FirstName = p.FirstName,
                    MiddleName = p.MiddleName,
                    LastName = p.LastName,
                    Status = p.Status,
                    ImageFull = p.ImageFull,
                    UrlName = p.UrlName
                }).ToList().OrderBy(p => p.Name);

            var playerIds = players.Select(p => p.Id);
            var memberTeams = _dbContext.MemberTeams.Where(mt => playerIds.Contains(mt.MemberId)).ToList();
            foreach (var player in players)
            {
                player.TeamIds = memberTeams.Where(mt => mt.MemberId == player.Id).Select(mt => mt.TeamId).ToList();
            }
            return players;

        }

        public void TogglePlayerTeam(Guid teamId, Guid playerId, Guid clubId)
        {

            var existingTeam = _dbContext.MemberTeams.FirstOrDefault(p => p.TeamId == teamId && p.MemberId == playerId);
            if (existingTeam != null)
            {
                _dbContext.Remove(existingTeam);
            }
            else
            {
                var memberTeam = new MemberTeam
                {
                    Id = Guid.NewGuid(),
                    MemberId = playerId,
                    TeamId = teamId
                };
                _dbContext.Add(memberTeam);
            }

            var userName = _dbContext.Members.Where(m => m.Id == playerId).Select(p => p.UserName).Single();

            _dbContext.SaveChanges();
            _cacheHelper.ClearCache(clubId, userName);
        }

        public ShowPlayerViewModel GetSingle(Guid clubId, string name)
        {
            var query = _dbContext.Players.Where(p => p.UrlName == name.ToLower() && p.ClubId == clubId);
            return GetPlayer(query);
        }

        public ShowPlayerViewModel GetSingle(Guid playerId)
        {
            var query = _dbContext.Players.Where(p => p.Id == playerId);
            return GetPlayer(query);
        }
        private ShowPlayerViewModel GetPlayer(IQueryable<Player> query)
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
                Status = p.Status,
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
                _dbContext.Players.Where(p => p.Status == status && p.ClubId == clubId)
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
                .Where(e => (e.PlayerId == playerId || e.AssistedById == playerId) && e.Game.GameType != GameType.Treningskamp)
                .ToList();

            var now = DateTime.Now;
            var games = _dbContext.Games.Where(
                          g => teamIds.Contains(g.TeamId) &&
                          g.GameType != GameType.Treningskamp &&
                          g.DateTime < now
                          && g.Attendees.Any(a => a.MemberId == playerId && a.IsSelected)
                          )
                .Select(g => new GameAttendanceViewModel
                {
                    TeamId = g.TeamId,
                    DateTime = g.DateTime
                }).ToList();



            var years = games
                .Select(g => new Key {TeamId = g.TeamId, Year = g.DateTime.Year})
                .Distinct()
                .Concat(teamIds.Select(tid => new Key {TeamId = tid, Year = 0}));

            
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
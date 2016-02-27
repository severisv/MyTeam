using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.Entity;
using MyTeam.Models;
using MyTeam.Models.Domain;
using MyTeam.Models.Dto;
using MyTeam.Models.Enums;
using MyTeam.Models.Structs;
using MyTeam.Resources;
using MyTeam.Services.Application;
using MyTeam.Services.Repositories;
using MyTeam.ViewModels.Game;
using MyTeam.ViewModels.Player;

namespace MyTeam.Services.Domain
{

    class PlayerService : IPlayerService
    {
        private readonly IRepository<Player> _playerRepository;
        private readonly IRepository<Club> _clubRepository;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly ICacheHelper _cacheHelper;

        public PlayerService(IRepository<Player> playerRepository, IRepository<Club> clubRepository, ApplicationDbContext applicationDbContext, ICacheHelper cacheHelper)
        {
            _playerRepository = playerRepository;
            _clubRepository = clubRepository;
            _cacheHelper = cacheHelper;
            _applicationDbContext = applicationDbContext;
        }

        public JsonResponseMessage Add(string clubId, string facebookId, string firstName, string middleName, string lastName, string emailAddress,
            string imageSmall, string imageMedium, string imageLarge)
        {
            var existingPlayer = string.IsNullOrWhiteSpace(facebookId) ?
                _playerRepository.Get().FirstOrDefault(p => p.UserName == emailAddress) :
                _playerRepository.Get().FirstOrDefault(p => p.FacebookId == facebookId);


            if (!string.IsNullOrWhiteSpace(facebookId) && string.IsNullOrWhiteSpace(emailAddress))
            {
                var correspondingUserLogin = _applicationDbContext.UserLogins.FirstOrDefault(u => u.ProviderKey == facebookId);
                if (correspondingUserLogin != null)
                {
                    emailAddress = _applicationDbContext.Users.Single(u => u.Id == correspondingUserLogin.UserId).Email;
                }
            }


            if (existingPlayer == null)
            {
                var club = _clubRepository.Get().Single(c => c.ClubIdentifier == clubId);

                var player = new Player
                {
                    FacebookId = facebookId,
                    FirstName = firstName,
                    LastName = lastName,
                    UserName = emailAddress,
                    ClubId = club.Id,
                    ImageSmall = imageSmall,
                    ImageMedium = imageMedium,
                    ImageFull = imageLarge,
                    MiddleName = middleName
                };

                _applicationDbContext.Players.Add(player);
                _playerRepository.CommitChanges();

                var message = string.IsNullOrWhiteSpace(facebookId)
                    ? $"{Res.Player} {Res.Added.ToLower()}"
                    : "facebookAdd";

                return JsonResponse.Success(message);
            }
            return JsonResponse.ValidationFailed($"{Res.Player} {Res.IsAlready.ToLower()} {Res.Added.ToLower()}");
        }


        public IEnumerable<string> GetFacebookIds()
        {
            return _playerRepository.Get().Select(p => p.FacebookId);
        }

        public void SetPlayerStatus(Guid id, PlayerStatus status, string clubName)
        {
            var player = _playerRepository.GetSingle(id);
            player.Status = status;
            _playerRepository.CommitChanges();
            _cacheHelper.ClearCache(clubName, player.UserName);
        }

        public void TogglePlayerRole(Guid id, string role, string clubName)
        {
            var player = _playerRepository.GetSingle(id);
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
            _playerRepository.CommitChanges();
            _cacheHelper.ClearCache(clubName, player.UserName);
        }

        public void EditPlayer(EditPlayerViewModel model, string clubId)
        {
            var player = _playerRepository.GetSingle(model.PlayerId);
            player.FirstName = model.FirstName;
            player.MiddleName = model.MiddleName;
            player.LastName = model.LastName;
            player.Phone = model.Phone;
            player.StartDate = model.StartDate.AsDate();
            player.BirthDate = model.BirthDate.AsDate();
            player.PositionsString = model.PositionsString;
            player.ProfileIsConfirmed = true;
            player.ImageFull = model.ImageFull;
            player.ImageMedium = model.ImageFull;
            player.ImageSmall = model.ImageFull;
            _applicationDbContext.SaveChanges();
            _cacheHelper.ClearCache(clubId, player.UserName);
        }

        public void AddEmailToPlayer(string facebookId, string email)
        {
            if (string.IsNullOrWhiteSpace(facebookId)) return;
            var players = _playerRepository.Get().Where(p => p.FacebookId == facebookId).ToList();
            foreach (var player in players)
            {
                if (string.IsNullOrWhiteSpace(player.UserName))
                {
                    player.UserName = email;
                }
            }
            _playerRepository.CommitChanges();
        }

        public IEnumerable<SimplePlayerDto> GetDto(Guid clubId)
        {
            var players = _playerRepository.Get().Where(p => p.ClubId == clubId)
                .Select(p => new SimplePlayerDto
                {
                    Id = p.Id,
                    FirstName = p.FirstName,
                    MiddleName = p.MiddleName,
                    LastName = p.LastName,
                    Status = p.Status,
                    ImageSmall = p.ImageSmall,
                }).ToList().OrderBy(p => p.Name);

            var playerIds = players.Select(p => p.Id);
            var memberTeams = _applicationDbContext.MemberTeams.Where(mt => playerIds.Contains(mt.MemberId)).ToList();
            foreach (var player in players)
            {
                player.TeamIds = memberTeams.Where(mt => mt.MemberId == player.Id).Select(mt => mt.TeamId).ToList();
            }
            return players;

        }

        public void TogglePlayerTeam(Guid teamId, Guid playerId, string clubName)
        {

            var existingTeam = _applicationDbContext.MemberTeams.FirstOrDefault(p => p.TeamId == teamId && p.MemberId == playerId);
            if (existingTeam != null)
            {
                _applicationDbContext.Remove(existingTeam);
            }
            else
            {
                var memberTeam = new MemberTeam
                {
                    Id = Guid.NewGuid(),
                    MemberId = playerId,
                    TeamId = teamId
                };
                _applicationDbContext.Add(memberTeam);
            }

            var userName = _applicationDbContext.Members.Where(m => m.Id == playerId).Select(p => p.UserName).Single();

            _applicationDbContext.SaveChanges();
            _cacheHelper.ClearCache(clubName, userName);

        }

        public ShowPlayerViewModel GetSingle(Guid playerId)
        {
            var player = _applicationDbContext.Players.Where(p => p.Id == playerId)
                .Select(p => new ShowPlayerViewModel
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
                    ImageMedium = p.ImageMedium,
                    PositionsString = p.PositionsString
                }).Single();

            var practiceCount =
                _applicationDbContext.EventAttendances.Count(e => e.MemberId == playerId && e.DidAttend && e.Event.DateTime.Year == DateTime.Now.Year);

            player.PracticeCount = practiceCount;

            return player;
        }

        public IEnumerable<ShowPlayerViewModel> GetPlayers(PlayerStatus status, Guid clubId)
        {
            return
                _applicationDbContext.Players.Where(p => p.Status == status && p.ClubId == clubId)
                    .OrderBy(p => p.FirstName)
                    .Select(p => new ShowPlayerViewModel
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
                        ImageMedium = p.ImageMedium,
                        PositionsString = p.PositionsString
                    }).ToList();
        }

        public IEnumerable<PlayerStatsViewModel> GetStats(Guid playerId, IEnumerable<Guid> teamIds)
        {
            var events = _applicationDbContext.GameEvents
                .Include(ge => ge.Game)
                .Where(e => (e.PlayerId == playerId || e.AssistedById == playerId) && e.Game.GameType != GameType.Treningskamp)
                .ToList();

            var games = _applicationDbContext.Games.Where(g => teamIds.Contains(g.TeamId) && g.GameType != GameType.Treningskamp)
                .Select(g => new GameAttendanceViewModel
                {
                    Attendances = g.Attendees.Count(a => a.MemberId == playerId && a.IsSelected),
                    TeamId = g.TeamId
                }).ToList();

            var grouped = events.GroupBy(e => e.Game.TeamId);

            return teamIds.Select(teamId =>
                new PlayerStatsViewModel(
                        playerId, teamId,
                            grouped.SingleOrDefault(g => g.Key == teamId)?.Select(ge => new GameEventViewModel
                            {
                                AssistedById = ge.AssistedById,
                                PlayerId = ge.PlayerId,
                                GameId = ge.Game.Id,
                                Type = ge.Type
                            }),
                             games.Where(g => g.TeamId == teamId).Sum(g => g.Attendances)
                    ))
                    .ToList()
                    .OrderByDescending(p => p.GameCount);


        }
    }
}
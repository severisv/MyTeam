using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Identity;
using MyTeam.Models;
using MyTeam.Models.Domain;
using MyTeam.Models.Dto;
using MyTeam.Models.Enums;
using MyTeam.Models.Structs;
using MyTeam.Resources;
using MyTeam.Services.Application;
using MyTeam.Services.Repositories;
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
                _playerRepository.Get().FirstOrDefault(p => p.UserName == emailAddress):
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
            else return JsonResponse.ValidationFailed($"{Res.Player} {Res.IsAlready.ToLower()} {Res.Added.ToLower()}");
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

        public void EditPlayer(EditPlayerViewModel model)
        {
            var player = _playerRepository.GetSingle(model.Id);
            player.FirstName = model.FirstName;
            player.MiddleName = model.MiddleName;
            player.LastName = model.LastName;
            player.Phone = model.Phone;
            player.StartDate = model.StartDate;
            player.BirthDate = model.BirthDate;
            player.PositionsString = model.PositionsString;
            _playerRepository.CommitChanges();
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

        public IEnumerable<SimplePlayerDto> GetDto(string clubId)
        {
            return _playerRepository.Get().Where(p => p.Club.ClubIdentifier == clubId).Select(p => new SimplePlayerDto()
            {
                Id = p.Id,
                Name = p.Fullname,
                Status =  p.Status,
                ImageSmall = p.ImageSmall
            });
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
    }
}
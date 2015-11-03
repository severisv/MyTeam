using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Identity;
using MyTeam.Models;
using MyTeam.Models.Domain;
using MyTeam.Models.Enums;
using MyTeam.Models.Structs;
using MyTeam.Resources;
using MyTeam.Services.Repositories;
using MyTeam.Settings;
using MyTeam.ViewModels.Admin;
using MyTeam.ViewModels.Player;

namespace MyTeam.Services.Domain
{

    class PlayerService : IPlayerService
    {
        private readonly IRepository<Player> _playerRepository;
        private readonly IRepository<Club> _clubRepository;

        public PlayerService(IRepository<Player> playerRepository, IRepository<Club> clubRepository)
        {
            _playerRepository = playerRepository;
            _clubRepository = clubRepository;
        }

        public JsonResponseMessage Add(string clubId, string facebookId, string firstName, string lastName, string emailAddress,
            string imageSmall, string imageMedium, string imageLarge)
        {
            var existingPlayer = string.IsNullOrWhiteSpace(facebookId) ?
                _playerRepository.Get().FirstOrDefault(p => p.UserName == emailAddress):
                _playerRepository.Get().FirstOrDefault(p => p.FacebookId == facebookId);

            if (existingPlayer == null)
            {
                var club = _clubRepository.Get().Single(c => c.ClubId == clubId);

                var player = new Player(
                    facebookId,
                    firstName,
                    lastName,
                    emailAddress
                    )
                {
                    ImageSmall = imageSmall,
                    ImageMedium = imageMedium,
                    ImageFull = imageLarge
                };

                player.Club = club;
                _playerRepository.Add(player);

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

        public IEnumerable<Object> Get(string clubId)
        {
            var players = _playerRepository.Get().Where(p => p.Club.ClubId == clubId).Select(p =>
            new
            {
                Id = p.Id,
                FullName = p.Name,
                Status = p.Status.ToString(),
                Roles = p.Roles
            });

           
            return players;
        }

        public void SetPlayerStatus(Guid id, PlayerStatus status)
        {
            var player = _playerRepository.Get(id).Single();
            player.Status = status;
        }

        public void TogglePlayerRole(Guid id, string role)
        {
            var player = _playerRepository.Get(id).Single();
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
        }

        public void EditPlayer(EditPlayerViewModel model)
        {
            var player = _playerRepository.Get(model.Id).Single();
            player.FirstName = model.FirstName;
            player.MiddleName = model.MiddleName;
            player.LastName = model.LastName;
            player.Phone = model.Phone;
            player.StartDate = model.StartDate;
            player.BirthDate = model.BirthDate;
        }
    }
}
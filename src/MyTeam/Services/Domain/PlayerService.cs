using System;
using System.Collections.Generic;
using System.Linq;
using MyTeam.Models.Domain;
using MyTeam.Models.Structs;
using MyTeam.Resources;
using MyTeam.Services.Repositories;
using MyTeam.Settings;
using MyTeam.ViewModels.Admin;

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
                FullName = p.Fullname,
                Status = p.Status.ToString()
            });
            return players;
        }
    }
}
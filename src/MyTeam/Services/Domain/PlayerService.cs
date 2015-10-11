using System.Linq;
using MyTeam.Models.Domain;
using MyTeam.Services.Repositories;

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

        public void Add(string clubId, string facebookId, string firstName, string lastName)
        {
            var existingPlayer = _playerRepository.Get().FirstOrDefault(p => p.FacebookId == facebookId);
            if (existingPlayer == null)
            {
                var club = _clubRepository.Get().Single(c => c.ClubId == clubId);

                var player = new Player(
                      facebookId,
                      firstName,
                      lastName
                );

                player.Club = club;
                 _playerRepository.Add(player);

            }
        }
    }
}
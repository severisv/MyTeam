using System.Collections.Generic;
using System.Linq;
using MyTeam.Models.Dto;
using MyTeam.Models.Enums;

namespace MyTeam.ViewModels.Game
{
    public class RegisterSquadViewModel
    {
        public RegisterSquadEventViewModel Game { get; }
        private readonly IEnumerable<RegisterSquadPlayerViewModel> _players;

        public IEnumerable<RegisterSquadPlayerViewModel> Attendees => _players.Where(p => Game.Attendees.Any(a => a.MemberId == p.Id && a.IsAttending == true));

        public IEnumerable<RegisterSquadPlayerViewModel> Declinees => _players.Where(p => Game.Attendees.Any(a => a.MemberId == p.Id && a.IsAttending == false));

        public IEnumerable<RegisterSquadPlayerViewModel> OtherActivePlayers => _players.Where(p => p.Status == PlayerStatus.Aktiv)
                                                                                        .Where(p => p.TeamIds.ContainsAny(Game.TeamIds))
                                                                                        .Where(p => p.Attendance?.IsAttending == null);

        public IEnumerable<RegisterSquadPlayerViewModel> OtherInactivePlayers
            =>
                _players.Where(p => !Attendees.Any(pl => pl.Id == p.Id))
                    .Where(p => !Declinees.Any(pl => pl.Id == p.Id))
                    .Where(p => !OtherActivePlayers.Any(pl => pl.Id == p.Id));

        public IEnumerable<RegisterSquadPlayerViewModel> Squad => _players.Where(p => p.Attendance?.IsSelected == true);

        public IEnumerable<RegisterSquadPlayerViewModel> Coaches { get; }

        public RegisterSquadViewModel(RegisterSquadEventViewModel game, IList<SimplePlayerDto> players)
        {
            Game = game;
            Coaches = players
                .Where(p => p.Status == PlayerStatus.Trener)
                .Where(p=> game.Attendees.Any(a => a.MemberId == p.Id))
                .Select(p => new RegisterSquadPlayerViewModel(p, Game.Id, game.Attendees.FirstOrDefault(a => a.MemberId == p.Id)
            ));

            _players = players
                .Where(p => p.Status != PlayerStatus.Trener)
                .Select(p => new RegisterSquadPlayerViewModel(p, Game.Id,
                    game.Attendees.FirstOrDefault(a => a.MemberId == p.Id)
                ));
        }
    }
}
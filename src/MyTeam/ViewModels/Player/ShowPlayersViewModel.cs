using System.Collections.Generic;
using MyTeam.Models.Enums;

namespace MyTeam.ViewModels.Player
{
    public class ShowPlayersViewModel
    {
        public ShowPlayerViewModel SelectedPlayer { get; }
        public IEnumerable<ListPlayerViewModel> Players { get; }
        public PlayerStatus Status { get; }

        public ShowPlayersViewModel(IEnumerable<ListPlayerViewModel> players, PlayerStatus status, ShowPlayerViewModel selectedPlayer = null)
        {
            Players = players;
            Status = status;
            SelectedPlayer = selectedPlayer;
        }

    }
}
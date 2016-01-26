using System;
using System.Collections.Generic;
using System.Linq;
using MyTeam.Models.Enums;

namespace MyTeam.ViewModels.Player
{
    public class ShowPlayersViewModel
    {

        public Guid? SelectedPlayerId { get; set; }
        public ShowPlayerViewModel SelectedPlayer => Players.SingleOrDefault(p => p.Id == SelectedPlayerId);
        public IEnumerable<ShowPlayerViewModel> Players { get; set; }
        public bool IsEditMode { get; set; }
        public PlayerStatus Status { get; set; }

        public ShowPlayersViewModel(IEnumerable<ShowPlayerViewModel> players, bool isEditMode, PlayerStatus status)
        {
            Players = players;
            IsEditMode = isEditMode;
            Status = status;
        }

    }
}
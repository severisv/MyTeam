using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MyTeam.Models.Enums;

namespace MyTeam.ViewModels.Player
{
    public class ShowPlayersViewModel
    {

        public Guid? SelectedPlayerId { get; set; }
        public Models.Domain.Player SelectedPlayer => Players.SingleOrDefault(p => p.Id == SelectedPlayerId);
        public IEnumerable<Models.Domain.Player> Players { get; set; }
        public bool IsEditMode { get; set; }
        public PlayerStatus Status { get; set; }

        public ShowPlayersViewModel(IQueryable<Models.Domain.Player> players, bool isEditMode, PlayerStatus status)
        {
            Players = players;
            IsEditMode = isEditMode;
            Status = status;
        }

    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MyTeam.ViewModels.Player
{
    public class ShowPlayersViewModel
    {

        public Guid? SelectedPlayerId { get; set; }
        public Models.Domain.Player SelectedPlayer => Players.SingleOrDefault(p => p.Id == SelectedPlayerId);
        public IEnumerable<Models.Domain.Player> Players { get; set; }
        public bool IsEditMode { get; set; }

        public ShowPlayersViewModel(IQueryable<Models.Domain.Player> players, bool isEditMode)
        {
            Players = players;
            IsEditMode = isEditMode;
        }

    }
}
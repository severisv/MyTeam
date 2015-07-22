using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MyTeam.ViewModels.Player
{
    public class ShowPlayersViewModel
    {
   
        public IEnumerable<Models.Domain.Player> Players { get; set; }

        public ShowPlayersViewModel(IQueryable<Models.Domain.Player> players)
        {
            Players = players;
        }

    }
}
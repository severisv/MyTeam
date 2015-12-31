using System;
using System.Collections.Generic;
using System.Linq;

namespace MyTeam.ViewModels.Game
{
    public class GameViewModel
    {
        public DateTime DateTime { get; set; }
        public string Opponent { get; set; }
        public IEnumerable<string> Teams { get; set; }
        public int Goals { get; set; }
        public int? HomeScore { get; set; }
        public int? AwayScore { get; set; }
        public bool IsHomeTeam { get; set; }
        public Guid Id { get; set; }
        public string Location { get; set; }

        public int? ActualHomeScore => IsHomeTeam ? HomeScore : AwayScore;
        public int? ActualAwayScore => IsHomeTeam ? AwayScore : HomeScore;
        public string HomeTeam => IsHomeTeam ? Teams.First() : Opponent;
        public string AwayTeam => IsHomeTeam ?  Opponent : Teams.First();
    }

}
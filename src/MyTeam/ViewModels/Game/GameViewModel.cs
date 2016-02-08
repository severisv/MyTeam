using System;
using System.Collections.Generic;
using System.Linq;
using MyTeam.Models.Enums;

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
        public GameType? GameType { get; set; }

        public int? ActualHomeScore => IsHomeTeam ? HomeScore : AwayScore;
        public int? ActualAwayScore => IsHomeTeam ? AwayScore : HomeScore;
        public string HomeTeam => IsHomeTeam ? Teams.First() : Opponent;
        public string AwayTeam => IsHomeTeam ?  Opponent : Teams.First();

        public string Outcome
        {
            get
            {
                var score = HomeScore - AwayScore;
                if (score == null) return "";
                if (score > 0) return IsHomeTeam ? "win" : "loss";
                if (score < 0) return !IsHomeTeam ? "win" : "loss";
                return "draw";
            }
        }

      
    }

}
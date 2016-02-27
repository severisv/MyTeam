using System;
using System.Linq;
using MyTeam.Models.Enums;
using MyTeam.Models.Shared;

namespace MyTeam.ViewModels.Game
{
    public class ParsedGame : IEvent
    {
        public Guid Id { get;  }
        public Guid TeamId { get;  }
        public DateTime DateTime { get;  }
        public string Opponent { get;  }
        public int? HomeScore { get;  }
        public int? AwayScore { get;  }
        public bool IsHomeTeam { get;  }
        public string Location { get;  }
        public GameType? GameType { get;  }
        public bool IsValid { get; }

        private readonly string _teamName;

        public ParsedGame(Guid teamId, string teamName, GameType gameType, string gameString)
        {
            TeamId = teamId;
            GameType = gameType;
            Id = Guid.NewGuid();
            _teamName = teamName;
      
            try
            {
                var fields = gameString.Split('\t');
                var startIndex = 0;
                if (fields[0].AsDate() == null) startIndex = 1;
                var date = fields[startIndex+0].AsDate().Value;
                var time = fields[startIndex+2].AsTime().Value;
                var homeTeam = fields[startIndex + 3];
                var awayTeam = fields[startIndex + 5];
                teamName = teamName.Substring(0, teamName.Length - 3);

                DateTime = date + time;
                IsHomeTeam = fields[startIndex+3].Contains(teamName);
                Opponent = IsHomeTeam ? awayTeam : homeTeam;

                var scoreArray = fields[startIndex + 4].Split(':');
                if (scoreArray.Length > 1)
                {
                    HomeScore = int.Parse(scoreArray[0]);
                    AwayScore = int.Parse(scoreArray[1]);
                }
                Location = fields[startIndex+6];
                IsValid = ThisIsValid(teamName, homeTeam, awayTeam);
            }
            catch (Exception)
            {
                IsValid = false;
            }
        }

        private bool ThisIsValid(string teamName, string homeTeam, string awayTeam)
        {
            return
                (homeTeam.Contains(teamName) || awayTeam.Contains(teamName)) &&
                !string.IsNullOrWhiteSpace(Opponent) &&
                !string.IsNullOrWhiteSpace(teamName) &&
                TeamId != Guid.Empty &&
                DateTime != DateTime.MinValue;

        }

        public string HomeTeam => IsHomeTeam ? _teamName : Opponent;
        public string AwayTeam => IsHomeTeam ? Opponent : _teamName;

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
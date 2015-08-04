using System;

namespace MyTeam.Models.Domain
{
    public class TableTeam
    {

        public TableTeam(string line)
        {
            try
            {
                var fields = line.Split(null);
                Name = fields[1];
                Position = P(fields[0]);
                Points = P(fields[22]);
                GoalsFor = P(fields[18]);
                GoalsAgainst = P(fields[20]);
                Wins = P(fields[15]);
                Draws = P(fields[16]);
                Losses = P(fields[17]);
         
            }
            catch (Exception)
            {
                Position = 0;
            }
        }

        private int P(string str)
        {
            return int.Parse(str);
        }

        public string Name { get; }
        public int Position { get; }
        public int Points { get;  }
        public int GoalsFor { get;  }
        public int GoalsAgainst { get;  }
        public int GoalDifference => GoalsFor - GoalsAgainst;
        public int GamesPlayed => Wins + Draws + Losses;
        public int Wins { get;  }
        public int Draws { get; }
        public int Losses { get; }
    }
}
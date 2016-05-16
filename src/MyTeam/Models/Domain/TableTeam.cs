using System.Text.RegularExpressions;

namespace MyTeam.Models.Domain
{
    public class TableTeam
    {

        public string Name { get; }
        public int Position { get; }
        public int Points { get; }
        public int GoalsFor { get; }
        public int GoalsAgainst { get; }
        public int GoalDifference => GoalsFor - GoalsAgainst;
        public int GamesPlayed => Wins + Draws + Losses;
        public int Wins { get; }
        public int Draws { get; }
        public int Losses { get; }

        public TableTeam(string line)
        {
                var fields = Regex.Split(line, ";");
                Position = P(fields[0]);
                Name = fields[1];
                Wins = P(fields[2]);
                Draws = P(fields[3]);
                Losses = P(fields[4]);
                GoalsFor = P(fields[5]);
                GoalsAgainst = P(fields[6]);
                Points = P(fields[7]);
        }
  

        private int P(string str)
        {
            return int.Parse(str);
        }


    }
}
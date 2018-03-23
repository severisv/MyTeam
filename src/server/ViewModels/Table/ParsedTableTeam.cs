using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace MyTeam.ViewModels.Table
{
    public class ParsedTableTeam
    {
        private const int ExpectedFieldCount = 23;
        private const int ExpectedNameIndex = 1;

        public ParsedTableTeam(string line)
        {
            try
            {
                var fields = Regex.Split(line.Trim(), @"\s+");
                
                if (fields.Length < ExpectedFieldCount)
                {
                    Position = -1;
                    return;
                }

                fields = RearrangeIfTeamNameHasWhiteSpace(fields);

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
                Position = -1;
            }
        }

        private string[] RearrangeIfTeamNameHasWhiteSpace(string[] fields)
        {
            
            var numberOfWhiteSpacesInTeamName = fields.Length - ExpectedFieldCount;

            if (numberOfWhiteSpacesInTeamName > 0)
            {
                var list = fields.ToList();
                var teamName = "";
                for (int i = ExpectedNameIndex; i <= ExpectedNameIndex + numberOfWhiteSpacesInTeamName; i++)
                {
                    teamName += fields[i] + " ";
                    list.RemoveAt(i);
                }
                list.Insert(ExpectedNameIndex, teamName.Trim());
                fields = list.ToArray();
            }
            return fields;
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
using System.Collections.Generic;
using System.Linq;

namespace MyTeam.ViewModels.Stats
{
    public class StatsViewModel
    {
        public int SelectedYear { get; }
        public IEnumerable<CurrentTeam> Teams  {get;}
        public string Team  {get;}
        public IEnumerable<int> Years  {get;}

        private IEnumerable<PlayerStats> _players;

        public IEnumerable<PlayerStats> Players => _players
            .OrderByDescending(p => p.Games)
            .ThenByDescending(p => p.Goals + p.Assists)
            .ThenByDescending(p => p.YellowCards)
            .ThenByDescending(p => p.RedCards);


        public StatsViewModel(IEnumerable<CurrentTeam> teams, string selectedTeamName, int selectedYear, IEnumerable<int> years, IEnumerable<PlayerStats> players)
        {
            Years = years;
            Teams = teams;
            Team = selectedTeamName;
            _players = players;
            SelectedYear = selectedYear;
        }

     
    }

}

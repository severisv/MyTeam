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

        public IEnumerable<PlayerStats> Goals => _players.Where(p => p.Goals > 0).OrderByDescending(p => p.Goals);
        public IEnumerable<PlayerStats> Assists => _players.Where(p => p.Assists > 0).OrderByDescending(p => p.Assists);
        public IEnumerable<PlayerStats> Cards => _players.Where(p => p.YellowCards > 0 || p.RedCards > 0).OrderByDescending(p => p.RedCards).ThenByDescending(p => p.YellowCards);
        public IEnumerable<PlayerStats> Games => _players.Where(p => p.Games > 0).OrderByDescending(p => p.Games);
        private readonly IEnumerable<PlayerStats> _players;

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

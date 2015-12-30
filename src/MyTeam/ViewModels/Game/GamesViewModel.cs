using System;
using System.Collections.Generic;
using System.Linq;
using MyTeam.ViewModels.Table;

namespace MyTeam.ViewModels.Game
{
    public class GamesViewModel
    {

        public IEnumerable<SeasonViewModel> Seasons { get; set; }
        public IList<TeamViewModel> Teams { get; set; }
        public Guid TeamId { get; set; }
        private readonly int _year;

        public SeasonViewModel SelectedSeason => Seasons.SingleOrDefault(s => s.Year == _year) ?? CurrentSeason;

        public SeasonViewModel CurrentSeason => Seasons.FirstOrDefault(s => s.Year == DateTime.Now.Year) ?? new SeasonViewModel
            {
                TeamId =TeamId,
                Year = DateTime.Now.Year
            };

        public IEnumerable<GameViewModel> Games { get; set; }

        public GamesViewModel(IEnumerable<SeasonViewModel> seasons, IList<TeamViewModel> teams, int year, IEnumerable<GameViewModel> games, Guid teamId)
        {
            Teams = teams;
            Seasons = seasons;
            _year = year;
            Games = games;
            TeamId = teamId;
        }

      
    }
}
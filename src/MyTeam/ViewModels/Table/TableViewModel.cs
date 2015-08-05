using System;
using System.Collections.Generic;
using System.Linq;
using MyTeam.Models.Domain;

namespace MyTeam.ViewModels.Table
{
    public class TableViewModel
    {
   
        public Models.Domain.Table Table { get; set; }
        public IEnumerable<Season> Seasons { get; set; }
        private readonly Guid? _selectedSeasonId;

        public Season SelectedSeason => Seasons.SingleOrDefault(s => s.Id == _selectedSeasonId) ?? CurrentSeason;

        public Season CurrentSeason=> Seasons.FirstOrDefault(s => s.StartDate.Date >= DateTime.Now.Date) ?? Seasons.FirstOrDefault();

        public TableViewModel(IEnumerable<Season> seasons, Guid? seasonId)
        {
            Seasons = seasons;
            _selectedSeasonId = seasonId;
        }
    }
}

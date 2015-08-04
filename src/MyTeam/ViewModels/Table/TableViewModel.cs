using System;
using System.Collections.Generic;
using System.Linq;
using MyTeam.Models.Domain;

namespace MyTeam.ViewModels.Table
{
    public class TableViewModel
    {
   
        public IEnumerable<Season> Seasons { get; set; }
        public Guid? SelectedSeasonId { get; set; }

        public Season SelectedSeason => Seasons.SingleOrDefault(s => s.Id == SelectedSeasonId) ?? CurrentSeason;

        public Season CurrentSeason=> Seasons.FirstOrDefault(s => s.StartDate.Date >= DateTime.Now.Date) ?? Seasons.FirstOrDefault();

        public TableViewModel(IEnumerable<Season> seasons, Guid? seasonId)
        {
            Seasons = seasons;
            SelectedSeasonId = seasonId;
        }
    }
}

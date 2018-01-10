﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace MyTeam.ViewModels.Table
{
    public class TableViewModel
    {

        public Models.Domain.Table Table => SelectedSeason?.Table;
        public IEnumerable<SeasonViewModel> Seasons { get; set; }
        public IList<TeamViewModel> Teams { get; set; }
        private readonly Guid? _selectedSeasonId;

        public SeasonViewModel SelectedSeason => Seasons.SingleOrDefault(s => s.Id == _selectedSeasonId) ?? CurrentSeason;

        public SeasonViewModel CurrentSeason => Seasons.FirstOrDefault(s => s.StartDate.Date >= DateTime.Now.Date) ?? Seasons.FirstOrDefault();
        public DateTime? TableUpdatedDate => SelectedSeason?.TableUpdated;

        public TableViewModel(IEnumerable<SeasonViewModel> seasons, IList<TeamViewModel>  teams, Guid? seasonId)
        {
            Teams = teams;
            Seasons = seasons;
            _selectedSeasonId = seasonId;
        }
    }
}
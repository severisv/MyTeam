using System;
using System.Collections.Generic;
using MyTeam.ViewModels.Table;

namespace MyTeam.Services.Domain
{
    public interface ISeasonService
    {
        IEnumerable<SeasonViewModel> GetForTeam(Guid teamId);
        IEnumerable<SeasonViewModel> GetTeamSeasonsFromSeasonId(Guid seasonId);
        void Delete(Guid seasonId);
        void Update(Guid seasonId, string name, bool autoUpdate, string sourceUrl);
    }
}
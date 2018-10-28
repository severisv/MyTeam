using System;
using System.Collections.Generic;
using MyTeam.ViewModels.Table;

namespace MyTeam.Services.Domain
{
    public interface ISeasonService
    {
        IEnumerable<SeasonViewModel> GetForTeam(Guid teamId);
        void Delete(Guid seasonId);
        void Update(Guid seasonId, string name, bool autoUpdate, string sourceUrl, bool autoUpdateFixtures, string fixturesSourceUrl);
        void Update(Guid seasonId, string name, bool autoUpdate, string sourceUrl);

        void CreateSeason(Guid teamId, int year, string name, bool autoUpdate, string sourceUrl, bool autoUpdateFixtures,
            string fixturesSourceUrl);


    }
}
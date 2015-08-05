using System;
using System.Collections.Generic;
using MyTeam.Models.Domain;

namespace MyTeam.Services.Domain
{
    public interface ISeasonService
    {
        Season Get(Guid id);
        IEnumerable<Season> GetForTeam(Guid teamId);
        IEnumerable<Season> GetTeamSeasonsFromSeasonId(Guid seasonId);
    }
}
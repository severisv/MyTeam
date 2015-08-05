using System;
using System.Collections.Generic;
using MyTeam.Models.Domain;

namespace MyTeam.Services.Domain
{
    public interface ISeasonService
    {
        IEnumerable<Season> Get(Guid teamId);
        IEnumerable<Season> GetTeamSeasonsFromSeasonId(Guid seasonId);
    }
}
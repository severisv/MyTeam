using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Mvc;
using MyTeam.Models.Domain;
using MyTeam.Services.Repositories;

namespace MyTeam.Services.Domain
{
    class SeasonService : ISeasonService
    {
        private readonly IRepository<Season> _seasonRepository;

        public SeasonService(IRepository<Season> seasonRepository)
        {
            _seasonRepository = seasonRepository;
        }

        public IEnumerable<Season> GetForTeam(Guid teamId)
        {
            return _seasonRepository.Get().Where(s => s.TeamId == teamId);
        }

        public Season Get(Guid seasonId)
        {
            return _seasonRepository.Get().Single(s => s.Id == seasonId);
        }

        public IEnumerable<Season> GetTeamSeasonsFromSeasonId(Guid seasonId)
        {
            var teamId = _seasonRepository.Get(seasonId).Select(s => s.Team.Id).Single();
            return GetForTeam(teamId);
        }
      
    }
}
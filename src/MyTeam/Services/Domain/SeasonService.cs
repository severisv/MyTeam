using System;
using System.Collections.Generic;
using System.Linq;
using MyTeam.Models;
using MyTeam.ViewModels.Table;

namespace MyTeam.Services.Domain
{
    class SeasonService : ISeasonService
    {
        private readonly ApplicationDbContext _dbContext;

        public SeasonService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<SeasonViewModel> GetForTeam(Guid teamId)
        {
            return _dbContext.Seasons
                .Where(s => s.TeamId == teamId)
                .Select(s =>
                  new SeasonViewModel
                  {
                      Id = s.Id,
                      StartDate = s.StartDate,
                      EndDate = s.EndDate,
                      Name = s.Name,
                      Team = new TeamViewModel
                      {
                          Id = s.TeamId,
                          Name = s.Team.Name
                      },
                      TeamId = teamId
                  }
                ).OrderByDescending(s => s.StartDate).ToList();
        }

    
        public IEnumerable<SeasonViewModel> GetTeamSeasonsFromSeasonId(Guid seasonId)
        {
            var teamId = _dbContext.Seasons.Where(s => s.Id == seasonId).Select(s => s.Team.Id).Single();
            return GetForTeam(teamId);
        }
      
    }
}
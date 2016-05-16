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
                      TeamId = teamId,
                      TableString = s.TableString
                  }
                ).OrderByDescending(s => s.StartDate).ToList();
        }

    
        public IEnumerable<SeasonViewModel> GetTeamSeasonsFromSeasonId(Guid seasonId)
        {
            var teamId = _dbContext.Seasons.Where(s => s.Id == seasonId).Select(s => s.Team.Id).Single();
            return GetForTeam(teamId);
        }

        public void Delete(Guid seasonId)
        {
            var season =_dbContext.Seasons.Single(s => s.Id == seasonId);
            _dbContext.Seasons.Remove(season);
            _dbContext.SaveChanges();
        }

        public void Update(Guid seasonId, string name, bool autoupdateTable, string tableSourceUrl = "")
        {
            var season = _dbContext.Seasons.Single(s => s.Id == seasonId);
            season.Name = name;
            season.AutoUpdateTable = autoupdateTable;
            season.TableSourceUrl = tableSourceUrl;
            _dbContext.SaveChanges();
        }
    }
}
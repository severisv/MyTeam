using System;
using System.Collections.Generic;
using System.Linq;
using MyTeam.Models;
using MyTeam.Models.Domain;
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
                      TableString = s.TableString,
                      TableUpdated = s.TableUpdated
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

        public void Update(Guid seasonId, string name, bool autoupdateTable, string tableSourceUrl, bool autoupdateFixtures, string fixturesSourceUrl)
        {
            var season = _dbContext.Seasons.Single(s => s.Id == seasonId);
            season.Name = name;
            season.AutoUpdateTable = autoupdateTable;
            season.TableSourceUrl = tableSourceUrl;
            season.AutoUpdateFixtures = autoupdateFixtures;
            season.FixturesSourceUrl = fixturesSourceUrl;
            _dbContext.SaveChanges();
        }

        public void Update(Guid seasonId, string name, bool autoupdateTable, string tableSourceUrl)
        {
            var season = _dbContext.Seasons.Single(s => s.Id == seasonId);
            season.Name = name;
            season.AutoUpdateTable = autoupdateTable;
            season.TableSourceUrl = tableSourceUrl;
            _dbContext.SaveChanges();
        }


        public void CreateSeason(Guid teamId, int year, string name, bool autoUpdate, string sourceUrl, bool autoUpdateFixtures, string fixturesSourceUrl)
        {
            var season = new Season
            {
                TeamId = teamId,
                Name = name,
                StartDate = new DateTime(year, 01, 01),
                EndDate = new DateTime(year, 12, 31, 23, 59, 59),
                AutoUpdateTable = autoUpdate,
                TableSourceUrl = sourceUrl,
                AutoUpdateFixtures = autoUpdateFixtures,
                FixturesSourceUrl = fixturesSourceUrl
            };
            _dbContext.Seasons.Add(season);
            _dbContext.SaveChanges();
        }
    }
}
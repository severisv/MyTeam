using System;
using System.Linq;
using MyTeam.Models;
using MyTeam.Models.Domain;

namespace MyTeam.Services.Domain
{
    class TableService : ITableService
    {
        private readonly ApplicationDbContext _dbContext;

        public TableService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        
        public Table GetTable(Guid seasonId)
        {
            return _dbContext.Tables.Where(t => t.SeasonId == seasonId).OrderByDescending(t => t.CreatedDate).FirstOrDefault();
             
        }

        public void Create(Guid seasonId, string tableString)
        {
            var table = new Table(seasonId, tableString);
            _dbContext.Add(table);
            _dbContext.SaveChanges();
        }

        public void CreateSeason(Guid teamId, int year, string name)
        {
            var season = new Season
            {
                TeamId = teamId,
                Name = name,
                StartDate = new DateTime(year, 01, 01),
                EndDate = new DateTime(year, 12, 31, 23,59,59)
            };
            _dbContext.Seasons.Add(season);
            _dbContext.SaveChanges();
        }
    }
}
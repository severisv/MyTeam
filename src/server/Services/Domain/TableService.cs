using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using HtmlAgilityPack;
using HtmlAgilityPack.CssSelectors.NetCore;
using Microsoft.Extensions.Logging;
using MyTeam.Models;
using MyTeam.Models.Domain;


namespace MyTeam.Services.Domain
{
    class TableService : ITableService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<TableService> _logger;

        public TableService(ApplicationDbContext dbContext, ILogger<TableService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }
        
        public void Update(Guid seasonId, string tableString)
        {
            var season = _dbContext.Seasons.Single(s => s.Id == seasonId);
            season.TableString = tableString;
            season.TableUpdated = DateTime.Now;
            _dbContext.SaveChanges();
        }         

    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;
using MyTeam.Models;
using MyTeam.Models.Domain;
using MyTeam.ViewModels.Table;
using ScrapySharp.Extensions;

namespace MyTeam.Services.Domain
{
    class TableService : ITableService
    {
        private readonly ApplicationDbContext _dbContext;

        public TableService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        
        public void Update(Guid seasonId, string tableString)
        {
            var season = _dbContext.Seasons.Single(s => s.Id == seasonId);
            season.TableString = tableString;
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

        public void RefreshTables()
        {
            var now = DateTime.Now;
            var seasons = _dbContext.Seasons.Where(s => s.StartDate <= now && s.EndDate >= now && s.AutoUpdateTable && !string.IsNullOrWhiteSpace(s.TableSourceUrl)).ToList();

            foreach (var season in seasons)
            {
                var tableString = ScrapeTable(season);
                Update(season.Id, tableString);
            }

        }

      



        private string ScrapeTable(Season season)
        {
            var web = new HtmlWeb();
            var doc = web.Load(season.TableSourceUrl);

            var table = doc.DocumentNode.CssSelect("table tr");

            var tableTeams = table.Select(t => t.CssSelect("td"));

            var tableStrings = tableTeams.Where(t => t.Any()).Select(GetTableTeamString);
            var tableString = string.Join("|", tableStrings);
            return tableString;
        }

        private string GetTableTeamString(IEnumerable<HtmlNode> htmlNodes)
        {
            var nodes = htmlNodes.ToArray();

           var maalforskjell =  nodes[14].InnerText.Split('-').Select(s => s.Trim()).ToArray();
            
            return string.Join(";", new []
            {
                nodes[0].InnerText,
                nodes[1].InnerText.Normalize(),
                nodes[11].InnerText,
                nodes[12].InnerText,
                nodes[13].InnerText,
                maalforskjell[0],
                maalforskjell[1],
                nodes[16].InnerText
            }.Select(Decode));

        }

        private string Decode(string str)
        {
            return HtmlEntity.DeEntitize(str).Trim();
        }
    }
}
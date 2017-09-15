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

      

        public void RefreshTables()
        {
            var now = DateTime.Now;
            var seasons = _dbContext.Seasons.Where(s => s.StartDate <= now && s.EndDate >= now && s.AutoUpdateTable && !string.IsNullOrWhiteSpace(s.TableSourceUrl)).ToList();

            foreach (var season in seasons)
            {
                try
                {
                    var tableString = ScrapeTable(season);
                    Update(season.Id, tableString);
                }
                catch (Exception e)
            {
                _logger.LogError(e, "Feil ved scraping av tabell. SeasonId: " + season.Id);
            }
        }
        }

        private string ScrapeTable(Season season)
        {
            var httpClient = new HttpClient();
            var htmlString = httpClient.GetAsync(season.TableSourceUrl).Result.Content.ReadAsStringAsync().Result;
            var doc = new HtmlDocument();
            doc.LoadHtml(htmlString);

            var table = doc.DocumentNode.QuerySelectorAll("table").First();
            var rows =  table.QuerySelectorAll("tr");

            var tableTeams = rows.Select(t => t.QuerySelectorAll("td"));

            var tableStrings = tableTeams.Where(IsValidRow).Select(GetTableTeamString);
            var tableString = string.Join("|", tableStrings);
            return tableString;
        }

        private bool IsValidRow(IEnumerable<HtmlNode> arg)
        {
            if (!arg?.Any() == true)
            {
                return false;
            }
            
            var nodes = arg.ToArray();

            // Sjekk at plasseringen er en int
            int pos;
            if (!int.TryParse(nodes[0].InnerText, out pos))
            {
                return false;
            }

            return true;
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
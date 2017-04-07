using System;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using MyTeam.Models;
using MyTeam.Models.Domain;
using ScrapySharp.Extensions;

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
           
                    var tableString = ScrapeTable(season);
                    Update(season.Id, tableString);
                //}
                //catch (Exception e)
                //{
                //    _logger.LogError("Feil ved scraping av tabell. SeasonId: " + season.Id, e);
                //}
            }
        }

        private string ScrapeTable(Season season)
        {
            var web = new HtmlWeb();
            var doc = web.Load(season.TableSourceUrl);

            var table = doc.DocumentNode.CssSelect("table").First();
            var rows =  table.CssSelect("tr");

            var tableTeams = rows.Select(t => t.CssSelect("td"));

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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyTeam.Models;
using MyTeam.Models.Domain;
using MyTeam.Models.Enums;
using HtmlAgilityPack.CssSelectors.NetCore;

namespace MyTeam.Services.Domain
{
    class FixtureService : IFixtureService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<TableService> _logger;

        public FixtureService(ApplicationDbContext dbContext, ILogger<TableService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public void RefreshFixtures()
        {
            var now = DateTime.Now;
            var seasons = _dbContext.Seasons.Where(s => s.StartDate <= now && s.EndDate >= now && s.AutoUpdateFixtures && !string.IsNullOrWhiteSpace(s.FixturesSourceUrl))
                .Include(s => s.Team)
                .ToList();

            foreach (var season in seasons)
            {
                try
                {
                    var games = ScrapeGames(season).Where(g => g.DateTime > DateTime.Now);
                    var existingGames =
                        _dbContext.Games.Where(g => g.TeamId == season.TeamId && g.DateTime > DateTime.Now && g.DateTime < season.EndDate && g.GameTypeValue == GameType.Seriekamp)
                        .ToList();

                    foreach (var game in games)
                    {
                        var existingGame = existingGames.FirstOrDefault(g => g.Opponent == game.Opponent && g.IsHomeTeam == game.IsHomeTeam);
                        if (existingGame != null)
                        {
                            existingGame.DateTime = game.DateTime;
                            existingGame.Location = game.Location;
                        }
                        else
                        {
                            _dbContext.Games.Add(game);
                        }
                    }
                    season.FixturesUpdated = DateTime.Now;
                    _dbContext.SaveChanges();

                }
                catch (Exception e)
                {
                    _logger.LogError(0, e, "Feil ved oppdatering av kamper av kamper. SeasonId: " + season.Id);
                }
            }
        }

        private IEnumerable<Game> ScrapeGames(Season season)
        {
            var httpClient = new HttpClient();
            var htmlString = httpClient.GetAsync(season.FixturesSourceUrl).Result.Content.ReadAsStringAsync().Result;
            var doc = new HtmlDocument();
            doc.LoadHtml(htmlString);


            var table = doc.DocumentNode.QuerySelectorAll("#matches table tr").ToList();
            if (!table.Any()) table = doc.DocumentNode.QuerySelectorAll("#matches-placeholder table tr").ToList();

            var header = table.Select(t => t.QuerySelectorAll("th")).First(h => h.Any());
            var indices = new FixtureTableIndex(header);

            var matches = table.Select(t => t.QuerySelectorAll("td"));

            var result = new List<Game>();
            foreach (var tr in matches.Where(tr => tr.Any()))
            {

                var parsedGame = GetGame(tr, season.TeamId, season.Team.ClubId, season.Team.Name, indices);
                if (parsedGame != null) result.Add(parsedGame);

            }
            return result;
        }

        private Game GetGame(IEnumerable<HtmlNode> htmlNodes, Guid teamId, Guid clubId, string teamName, FixtureTableIndex indices)
        {
            var nodes = htmlNodes.Select(n => Decode(n.InnerText)).ToArray();
            var eventId = Guid.NewGuid();
            var dateTime = nodes[indices.Date].AsDate() + nodes[indices.Time].AsTime();
            var location = nodes[indices.Location];
            var homeTeam = nodes[indices.HomeTeam];
            var awayTeam = nodes[indices.AwayTeam];
            var isHomeTeam = teamName.Contains(homeTeam);
            var opponent = isHomeTeam ? awayTeam : homeTeam;

            if (!(teamName.Contains(homeTeam) || teamName.Contains(awayTeam))) return null;

            return new Game
            {
                Id = eventId,
                DateTime = dateTime.Value,
                IsHomeTeam = isHomeTeam,
                Location = location,
                Opponent = opponent,
                TeamId = teamId,
                GameType = (int?)GameType.Seriekamp,
                Type = EventType.Kamp,
                ClubId = clubId,
                EventTeams = new List<EventTeam>
                {
                    new EventTeam
                    {
                        Id = Guid.NewGuid(),
                        TeamId = teamId,
                        EventId = eventId
                    }
                }
            };
        }


        private string Decode(string str)
        {
            return HtmlEntity.DeEntitize(str).Trim();
        }
    }

    internal class FixtureTableIndex
    {

        public int Time => _headers.FindIndex(h => h.InnerText?.ToLower()?.Contains("tid") == true);
        public int Date => _headers.FindIndex(h => h.InnerText?.ToLower()?.Contains("dato") == true);
        public int Location => _headers.FindIndex(h => h.InnerText?.ToLower()?.Contains("bane") == true);
        public int HomeTeam => _headers.FindIndex(h => h.InnerText?.ToLower()?.Contains("hjemmelag") == true);
        public int AwayTeam => _headers.FindIndex(h => h.InnerText?.ToLower()?.Contains("bortelag") == true);

        private readonly List<HtmlNode> _headers;

        public FixtureTableIndex(IEnumerable<HtmlNode> headers)
        {
            _headers = headers.ToList();
        }


    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyTeam.Models;
using MyTeam.Models.Domain;
using MyTeam.Models.Enums;
using ScrapySharp.Extensions;

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
                    var existingGames = _dbContext.Games.Where(g => g.TeamId == season.TeamId && g.DateTime > DateTime.Now && g.DateTime < season.EndDate && g.GameType == GameType.Seriekamp);

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
                    _logger.LogError("Feil ved oppdatering av kamper av kamper. SeasonId: " + season.Id, e);
                }
            }
        }

        private IEnumerable<Game> ScrapeGames(Season season)
        {
            var web = new HtmlWeb();
            var doc = web.Load(season.FixturesSourceUrl);

            var table = doc.DocumentNode.CssSelect("#matches table tr");

            var matches = table.Select(t => t.CssSelect("td"));

            var result = new List<Game>();
            foreach (var tr in matches.Where(tr => tr.Count() > 7))
            {
                try
                {
                    var parsedGame = GetGame(tr, season.TeamId, season.Team.ClubId, season.Team.Name);
                    if(parsedGame != null) result.Add(parsedGame);
                }
                catch (Exception e)
                {
                    _logger.LogError("Feil ved parsing av node: " + tr, e);
                }
            }
            return result;
        }

        private Game GetGame(IEnumerable<HtmlNode> htmlNodes, Guid teamId, Guid clubId, string teamName)
        {
            var nodes = htmlNodes.Select(n => Decode(n.InnerText)).ToArray();
            var eventId = Guid.NewGuid();
            var dateTime = nodes[0].AsDate() + nodes[2].AsTime();
            var location = nodes[7];
            var homeTeam = nodes[4];
            var awayTeam = nodes[6];
            var isHomeTeam = teamName.Contains(homeTeam);
            var opponent = isHomeTeam ? awayTeam : homeTeam;

            if(!(teamName.Contains(homeTeam) || teamName.Contains(awayTeam))) return null;

            return new Game
            {
                Id = eventId,
                DateTime = dateTime.Value,
                IsHomeTeam = isHomeTeam,
                Location = location,
                Opponent = opponent,
                TeamId = teamId,
                GameType = GameType.Seriekamp,
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
}
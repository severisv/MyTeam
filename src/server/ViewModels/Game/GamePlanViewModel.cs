using System;
using System.Collections.Generic;
using System.Linq;
using MyTeam.Models.Dto;
using MyTeam.Settings;
using Newtonsoft.Json;

namespace MyTeam.ViewModels.Game
{
    public class GamePlanViewModel
    {
        public Guid GameId { get; }
        public string GamePlan { get; }
        public string Team { get; }
        public string Opponent { get; }
        public bool IsPublished { get; }

        public string Players { get; }

        public GamePlanViewModel(Guid gameId, string team, string opponent, string gamePlan, bool? isPublished, IEnumerable<SquadMember> players, Cloudinary cloudinary)
        {
            GameId = gameId;
            GamePlan = gamePlan;
            Team = team;
            Opponent = opponent;
            IsPublished = isPublished ?? false;
            Players = JsonConvert.SerializeObject(
                players.OrderBy(p => p.FirstName)
                    .Select(p => new {
                        Id = p.Id,
                        Name = p.GetName(players.Count(ip => ip.FirstName == p.FirstName) > 1),
                        ImageUrl = cloudinary.MemberImage(p.Image, p.FacebookId, 40, 40)
                    })
                );
        }
    }
}
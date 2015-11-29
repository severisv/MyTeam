using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.Entity.Storage;
using MyTeam.Models;
using MyTeam.Models.Domain;
using MyTeam.Models.Enums;

namespace MyTeam.Services.Repositories
{
    public static class BootstrapData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {

            var context = serviceProvider.GetService(typeof(ApplicationDbContext)) as ApplicationDbContext;



            var club = new Club()
            {
                Id = new Guid("6790dd24-cf7f-442d-bec7-1a8e7f792a33"),
                ShortName = "Wam-Kam",
                Name = "Wam-Kam FK",
                ClubIdentifier = "wamkam",
                Logo = "image/upload/v1448650992/wamkam/wamkam.png",
                Favicon = "image/upload/v1448650991/wamkam/favicon.png",
            };


            if (!context.Clubs.Any())
                {

                    context.Clubs.Add(club);

                    context.SaveChanges();
               }

            if (!context.Teams.Any())
            {

                var teams = new List<Team>
                {
                    new Team
                    {
                        Id = Guid.NewGuid(),
                        ClubId = club.Id,
                        Name = "Wam-Kam 2",
                        SortOrder = 2
                    },
                    new Team
                    {
                        Id = Guid.NewGuid(),
                        ClubId = club.Id,
                        Name = "Wam-Kam 1",
                        SortOrder = 1
                    }
                };
                    context.Teams.AddRange(teams);
                    context.SaveChanges();
               }

            if (!context.Players.Any())
            {
                var player = new Player
                {
                    ClubId = club.Id,
                    FirstName = "Severin",
                    LastName = "Sverdvik",
                    Status = PlayerStatus.Aktiv,
                    UserName = "severin@sverdvik.no",
                    PositionsString = $"{Position.Back}",
                    RolesString = Roles.Admin,
                    Phone = "91611166"
                };
                context.Members.AddRange(player);
                context.SaveChanges();
            }
        }
    }
}
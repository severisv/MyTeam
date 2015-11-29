using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Data.Entity;
using MyTeam.Models.Domain;

namespace MyTeam.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Article> Articles { get; set; }
        public DbSet<Club> Clubs { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<EventAttendance> EventAttendances { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<Season> Seasons { get; set; }
        public DbSet<Table> Tables { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Training> Trainings { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }
    }
}

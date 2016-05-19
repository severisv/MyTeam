﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MyTeam.Models.Domain;
using MyTeam.ViewModels.Table;

namespace MyTeam.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Article> Articles { get; set; }
        public DbSet<Club> Clubs { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<GameEvent> GameEvents { get; set; }
        public DbSet<EventTeam> EventTeams { get; set; }
        public DbSet<EventAttendance> EventAttendances { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<MemberTeam> MemberTeams { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<Season> Seasons { get; set; }
        public DbSet<Season> Squads { get; set; }
        public DbSet<Team> Teams { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            foreach (var entity in builder.Model.GetEntityTypes())
            {
                entity.Relational().TableName = entity.DisplayName();
            }

            builder.Entity<Member>()
                .HasMany(e => e.EventAttendances)
                .WithOne(c => c.Member)
                .HasForeignKey(c => c.MemberId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Team>()
                .HasMany(e => e.MemberTeams)
                .WithOne(c => c.Team)
                .HasForeignKey(c => c.TeamId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Team>()
                .HasMany(e => e.EventTeams)
                .WithOne(c => c.Team)
                .HasForeignKey(c => c.TeamId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Team>()
                .HasMany(e => e.Games)
                .WithOne(c => c.Team)
                .HasForeignKey(c => c.TeamId)
                .OnDelete(DeleteBehavior.Restrict);
            
            builder.Entity<GameEvent>()
               .HasOne(e => e.Player)
               .WithMany(c => c.GameEvents)
               .HasForeignKey(c => c.PlayerId)
               .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<GameEvent>()
               .HasOne(e => e.AssistedBy)
               .WithMany(c => c.Assists)
               .HasForeignKey(c => c.AssistedById)
               .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Member>()
              .HasOne(p => p.Club)
              .WithMany(b => b.Members)
              .HasForeignKey(p => p.ClubId);


        }
    }
}

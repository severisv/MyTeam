using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using MyTeam.Models.Domain;
using Newtonsoft.Json;
using MyTeam.Models.Enums;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;

namespace MyTeam.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>, IDataProtectionKeyContext
    {
        public DbSet<Article> Articles { get; set; }
        public DbSet<Club> Clubs { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Event> Events { get; set; }

        [NotMapped] public virtual IQueryable<Event> Games => Events.Where(ev => ev.Type == (int)EventType.Kamp);
        public DbSet<GameEvent> GameEvents { get; set; }
        public DbSet<EventTeam> EventTeams { get; set; }
        public DbSet<EventAttendance> EventAttendances { get; set; }
        public DbSet<Fine> Fines { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<MemberTeam> MemberTeams { get; set; }
        public DbSet<PaymentInformation> PaymentInformation { get; set; }
        public DbSet<RemedyRate> RemedyRates { get; set; }
        public DbSet<Season> Seasons { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<MemberRequest> MemberRequests { get; set; }

        public DbSet<DataProtectionKey> DataProtectionKeys { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);


            builder.Entity<Article>().ToTable("Article");
            builder.Entity<Club>().ToTable("Club");
            builder.Entity<Comment>().ToTable("Comment");
            builder.Entity<Event>().ToTable("Event");
            builder.Entity<GameEvent>().ToTable("GameEvent");
            builder.Entity<EventTeam>().ToTable("EventTeam");
            builder.Entity<EventAttendance>().ToTable("EventAttendance");
            builder.Entity<Member>().ToTable("Member");
            builder.Entity<MemberTeam>().ToTable("MemberTeam");
            builder.Entity<Season>().ToTable("Season");
            builder.Entity<Member>().ToTable("Member");
            builder.Entity<Team>().ToTable("Team");


            builder.Entity<Member>()
                .HasMany(e => e.EventAttendances)
                .WithOne(c => c.Member)
                .HasForeignKey(c => c.MemberId)
                .OnDelete(DeleteBehavior.Restrict);


            builder.Entity<Member>()
                .HasMany(e => e.Payments)
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


            builder.Entity<Comment>()
                .HasOne(m => m.Member)
                .WithMany(c => c.Comments)
                .IsRequired(false);


            builder.Entity<Article>()
                .HasOne(a => a.Game)
                .WithOne(g => g.Report)
                .IsRequired(false);

        }
    }




    public class MainDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var connectionString = ReadConnectionStringFromAppsettings();

            var builder = new DbContextOptionsBuilder<ApplicationDbContext>();

            builder.UseSqlServer(connectionString);

            return new ApplicationDbContext(builder.Options);
        }

        private static string ReadConnectionStringFromAppsettings()
        {
            var appsettingsPath = $"{Directory.GetCurrentDirectory()}/appsettings.Development.json".Replace("database", "server");

            if (File.Exists(appsettingsPath))
            {
                using (var r = new StreamReader(appsettingsPath))
                {
                    var appsettingsString = r.ReadToEnd();
                    var appsettings = JsonConvert.DeserializeObject<Appsettings>(appsettingsString);
                    return appsettings.ConnectionStrings.DefaultConnection;
                }
            }
            else
            {
                return System.Environment.GetEnvironmentVariable("ConnectionStrings:DefaultConnection");
            }
        }


    }
    class Appsettings
    {
        public ConnectionStrings ConnectionStrings { get; set; }
    }

    class ConnectionStrings
    {
        public string DefaultConnection { get; set; }
    }


}
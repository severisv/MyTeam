using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using MyTeam.Models;

namespace MyTeam.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20160225111920_clubdescription")]
    partial class clubdescription
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.0-rc1-16348")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityRole", b =>
                {
                    b.Property<string>("Id");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Name")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<string>("NormalizedName")
                        .HasAnnotation("MaxLength", 256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .HasAnnotation("Relational:Name", "RoleNameIndex");

                    b.HasAnnotation("Relational:TableName", "AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("RoleId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasAnnotation("Relational:TableName", "AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasAnnotation("Relational:TableName", "AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasAnnotation("Relational:TableName", "AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasAnnotation("Relational:TableName", "AspNetUserRoles");
                });

            modelBuilder.Entity("MyTeam.Models.ApplicationUser", b =>
                {
                    b.Property<string>("Id");

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Email")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("NormalizedEmail")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<string>("NormalizedUserName")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("SecurityStamp");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .HasAnnotation("MaxLength", 256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasAnnotation("Relational:Name", "EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .HasAnnotation("Relational:Name", "UserNameIndex");

                    b.HasAnnotation("Relational:TableName", "AspNetUsers");
                });

            modelBuilder.Entity("MyTeam.Models.Domain.Article", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid?>("AuthorId");

                    b.Property<Guid>("ClubId");

                    b.Property<string>("Content")
                        .IsRequired();

                    b.Property<Guid?>("GameId");

                    b.Property<string>("Headline")
                        .IsRequired();

                    b.Property<string>("ImageUrl");

                    b.Property<DateTime>("Published");

                    b.HasKey("Id");
                });

            modelBuilder.Entity("MyTeam.Models.Domain.Club", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClubIdentifier")
                        .IsRequired();

                    b.Property<string>("Description");

                    b.Property<string>("Favicon");

                    b.Property<string>("Logo")
                        .IsRequired();

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<string>("ShortName")
                        .IsRequired();

                    b.HasKey("Id");
                });

            modelBuilder.Entity("MyTeam.Models.Domain.Comment", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("ArticleId");

                    b.Property<string>("AuthorImage");

                    b.Property<string>("AuthorName");

                    b.Property<string>("AuthorUserName");

                    b.Property<string>("Content");

                    b.Property<DateTime>("Date");

                    b.Property<Guid?>("MemberId");

                    b.HasKey("Id");
                });

            modelBuilder.Entity("MyTeam.Models.Domain.Event", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("ClubId");

                    b.Property<DateTime>("DateTime");

                    b.Property<string>("Description");

                    b.Property<string>("Discriminator")
                        .IsRequired();

                    b.Property<int?>("GameType");

                    b.Property<string>("Headline");

                    b.Property<bool>("IsHomeTeam");

                    b.Property<bool>("IsPublished");

                    b.Property<string>("Location")
                        .IsRequired();

                    b.Property<string>("Opponent");

                    b.Property<int>("Type");

                    b.Property<bool>("Voluntary");

                    b.HasKey("Id");

                    b.HasAnnotation("Relational:DiscriminatorProperty", "Discriminator");

                    b.HasAnnotation("Relational:DiscriminatorValue", "Event");
                });

            modelBuilder.Entity("MyTeam.Models.Domain.EventAttendance", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("DidAttend");

                    b.Property<Guid>("EventId");

                    b.Property<bool>("IsAttending");

                    b.Property<bool>("IsSelected");

                    b.Property<Guid>("MemberId");

                    b.Property<string>("SignupMessage");

                    b.Property<bool>("WonTraining");

                    b.HasKey("Id");
                });

            modelBuilder.Entity("MyTeam.Models.Domain.EventTeam", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("EventId");

                    b.Property<Guid>("TeamId");

                    b.HasKey("Id");
                });

            modelBuilder.Entity("MyTeam.Models.Domain.GameEvent", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid?>("AssistedById");

                    b.Property<DateTime>("CreatedDate");

                    b.Property<Guid>("GameId");

                    b.Property<Guid?>("PlayerId");

                    b.Property<int>("Type");

                    b.HasKey("Id");
                });

            modelBuilder.Entity("MyTeam.Models.Domain.Member", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime?>("BirthDate");

                    b.Property<Guid>("ClubId");

                    b.Property<string>("Discriminator")
                        .IsRequired();

                    b.Property<string>("FacebookId");

                    b.Property<string>("FirstName")
                        .IsRequired();

                    b.Property<string>("ImageFull");

                    b.Property<string>("ImageMedium");

                    b.Property<string>("ImageSmall");

                    b.Property<string>("Imagename");

                    b.Property<string>("LastName")
                        .IsRequired();

                    b.Property<string>("MiddleName");

                    b.Property<string>("Phone");

                    b.Property<bool>("ProfileIsConfirmed");

                    b.Property<string>("RolesString");

                    b.Property<DateTime?>("StartDate");

                    b.Property<string>("UserName");

                    b.HasKey("Id");

                    b.HasAnnotation("Relational:DiscriminatorProperty", "Discriminator");

                    b.HasAnnotation("Relational:DiscriminatorValue", "Member");
                });

            modelBuilder.Entity("MyTeam.Models.Domain.MemberTeam", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("MemberId");

                    b.Property<Guid>("TeamId");

                    b.HasKey("Id");
                });

            modelBuilder.Entity("MyTeam.Models.Domain.Season", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("EndDate");

                    b.Property<string>("Name");

                    b.Property<DateTime>("StartDate");

                    b.Property<Guid>("TeamId");

                    b.HasKey("Id");
                });

            modelBuilder.Entity("MyTeam.Models.Domain.Table", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedDate");

                    b.Property<Guid>("SeasonId");

                    b.Property<string>("TableString")
                        .IsRequired();

                    b.HasKey("Id");
                });

            modelBuilder.Entity("MyTeam.Models.Domain.Team", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("ClubId");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<string>("ShortName")
                        .IsRequired();

                    b.Property<int>("SortOrder");

                    b.HasKey("Id");
                });

            modelBuilder.Entity("MyTeam.Models.Domain.Game", b =>
                {
                    b.HasBaseType("MyTeam.Models.Domain.Event");

                    b.Property<int?>("AwayScore");

                    b.Property<int?>("HomeScore");

                    b.HasAnnotation("Relational:DiscriminatorValue", "Game");
                });

            modelBuilder.Entity("MyTeam.Models.Domain.Player", b =>
                {
                    b.HasBaseType("MyTeam.Models.Domain.Member");

                    b.Property<int>("AssistCount");

                    b.Property<int>("GameCount");

                    b.Property<int>("GoalCount");

                    b.Property<string>("PositionsString");

                    b.Property<int>("Status");

                    b.HasAnnotation("Relational:DiscriminatorValue", "Player");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNet.Identity.EntityFramework.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("MyTeam.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("MyTeam.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNet.Identity.EntityFramework.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId");

                    b.HasOne("MyTeam.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("MyTeam.Models.Domain.Article", b =>
                {
                    b.HasOne("MyTeam.Models.Domain.Member")
                        .WithMany()
                        .HasForeignKey("AuthorId");

                    b.HasOne("MyTeam.Models.Domain.Club")
                        .WithMany()
                        .HasForeignKey("ClubId");

                    b.HasOne("MyTeam.Models.Domain.Game")
                        .WithOne()
                        .HasForeignKey("MyTeam.Models.Domain.Article", "GameId");
                });

            modelBuilder.Entity("MyTeam.Models.Domain.Comment", b =>
                {
                    b.HasOne("MyTeam.Models.Domain.Article")
                        .WithMany()
                        .HasForeignKey("ArticleId");

                    b.HasOne("MyTeam.Models.Domain.Member")
                        .WithMany()
                        .HasForeignKey("MemberId");
                });

            modelBuilder.Entity("MyTeam.Models.Domain.Event", b =>
                {
                    b.HasOne("MyTeam.Models.Domain.Club")
                        .WithMany()
                        .HasForeignKey("ClubId");
                });

            modelBuilder.Entity("MyTeam.Models.Domain.EventAttendance", b =>
                {
                    b.HasOne("MyTeam.Models.Domain.Event")
                        .WithMany()
                        .HasForeignKey("EventId");

                    b.HasOne("MyTeam.Models.Domain.Member")
                        .WithMany()
                        .HasForeignKey("MemberId");
                });

            modelBuilder.Entity("MyTeam.Models.Domain.EventTeam", b =>
                {
                    b.HasOne("MyTeam.Models.Domain.Event")
                        .WithMany()
                        .HasForeignKey("EventId");

                    b.HasOne("MyTeam.Models.Domain.Team")
                        .WithMany()
                        .HasForeignKey("TeamId");
                });

            modelBuilder.Entity("MyTeam.Models.Domain.GameEvent", b =>
                {
                    b.HasOne("MyTeam.Models.Domain.Player")
                        .WithMany()
                        .HasForeignKey("AssistedById");

                    b.HasOne("MyTeam.Models.Domain.Game")
                        .WithMany()
                        .HasForeignKey("GameId");

                    b.HasOne("MyTeam.Models.Domain.Player")
                        .WithMany()
                        .HasForeignKey("PlayerId");
                });

            modelBuilder.Entity("MyTeam.Models.Domain.Member", b =>
                {
                    b.HasOne("MyTeam.Models.Domain.Club")
                        .WithMany()
                        .HasForeignKey("ClubId");
                });

            modelBuilder.Entity("MyTeam.Models.Domain.MemberTeam", b =>
                {
                    b.HasOne("MyTeam.Models.Domain.Member")
                        .WithMany()
                        .HasForeignKey("MemberId");

                    b.HasOne("MyTeam.Models.Domain.Team")
                        .WithMany()
                        .HasForeignKey("TeamId");
                });

            modelBuilder.Entity("MyTeam.Models.Domain.Season", b =>
                {
                    b.HasOne("MyTeam.Models.Domain.Team")
                        .WithMany()
                        .HasForeignKey("TeamId");
                });

            modelBuilder.Entity("MyTeam.Models.Domain.Table", b =>
                {
                    b.HasOne("MyTeam.Models.Domain.Season")
                        .WithMany()
                        .HasForeignKey("SeasonId");
                });

            modelBuilder.Entity("MyTeam.Models.Domain.Team", b =>
                {
                    b.HasOne("MyTeam.Models.Domain.Club")
                        .WithMany()
                        .HasForeignKey("ClubId");
                });

            modelBuilder.Entity("MyTeam.Models.Domain.Game", b =>
                {
                });

            modelBuilder.Entity("MyTeam.Models.Domain.Player", b =>
                {
                });
        }
    }
}

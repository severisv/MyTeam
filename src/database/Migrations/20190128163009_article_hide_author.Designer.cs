﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MyTeam.Models;

namespace MyTeam.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20190128163009_article_hide_author")]
    partial class article_hide_author
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.4-rtm-31024")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Name")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("RoleId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("LoginProvider");

                    b.Property<string>("Name");

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("MyTeam.Models.ApplicationUser", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Email")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("SecurityStamp");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers");
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

                    b.Property<bool>("HideAuthor");

                    b.Property<string>("ImageUrl");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<DateTime>("Published");

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.HasIndex("ClubId");

                    b.HasIndex("GameId")
                        .IsUnique()
                        .HasFilter("[GameId] IS NOT NULL");

                    b.ToTable("Article");
                });

            modelBuilder.Entity("MyTeam.Models.Domain.Club", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("About");

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

                    b.Property<string>("Sponsors");

                    b.HasKey("Id");

                    b.ToTable("Club");
                });

            modelBuilder.Entity("MyTeam.Models.Domain.Comment", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("ArticleId");

                    b.Property<string>("AuthorFacebookId");

                    b.Property<string>("AuthorName");

                    b.Property<string>("AuthorUserName");

                    b.Property<string>("Content");

                    b.Property<DateTime>("Date");

                    b.Property<Guid?>("MemberId");

                    b.HasKey("Id");

                    b.HasIndex("ArticleId");

                    b.HasIndex("MemberId");

                    b.ToTable("Comment");
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

                    b.Property<bool?>("GamePlanIsPublished");

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

                    b.HasIndex("ClubId");

                    b.ToTable("Event");

                    b.HasDiscriminator<string>("Discriminator").HasValue("Event");
                });

            modelBuilder.Entity("MyTeam.Models.Domain.EventAttendance", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("DidAttend");

                    b.Property<Guid>("EventId");

                    b.Property<bool?>("IsAttending")
                        .IsRequired();

                    b.Property<bool>("IsSelected");

                    b.Property<Guid>("MemberId");

                    b.Property<string>("SignupMessage");

                    b.Property<bool>("WonTraining");

                    b.HasKey("Id");

                    b.HasIndex("EventId");

                    b.HasIndex("MemberId");

                    b.ToTable("EventAttendance");
                });

            modelBuilder.Entity("MyTeam.Models.Domain.EventTeam", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("EventId");

                    b.Property<Guid>("TeamId");

                    b.HasKey("Id");

                    b.HasIndex("EventId");

                    b.HasIndex("TeamId");

                    b.ToTable("EventTeam");
                });

            modelBuilder.Entity("MyTeam.Models.Domain.Fine", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("Amount");

                    b.Property<string>("Comment");

                    b.Property<DateTime>("Issued");

                    b.Property<Guid>("MemberId");

                    b.Property<string>("RateName");

                    b.Property<Guid>("RemedyRateId");

                    b.HasKey("Id");

                    b.HasIndex("MemberId");

                    b.HasIndex("RemedyRateId");

                    b.ToTable("Fines");
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

                    b.HasIndex("AssistedById");

                    b.HasIndex("GameId");

                    b.HasIndex("PlayerId");

                    b.ToTable("GameEvent");
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

                    b.Property<string>("LastName")
                        .IsRequired();

                    b.Property<string>("MiddleName");

                    b.Property<string>("Phone");

                    b.Property<bool>("ProfileIsConfirmed");

                    b.Property<string>("RolesString");

                    b.Property<DateTime?>("StartDate");

                    b.Property<int>("Status");

                    b.Property<string>("UrlName");

                    b.Property<string>("UserName");

                    b.HasKey("Id");

                    b.HasIndex("ClubId");

                    b.ToTable("Member");

                    b.HasDiscriminator<string>("Discriminator").HasValue("Member");
                });

            modelBuilder.Entity("MyTeam.Models.Domain.MemberRequest", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<Guid>("ClubId");

                    b.Property<string>("Email");

                    b.Property<string>("FacebookId");

                    b.Property<string>("FirstName");

                    b.Property<string>("LastName");

                    b.Property<string>("MiddleName");

                    b.HasKey("Id");

                    b.ToTable("MemberRequests");
                });

            modelBuilder.Entity("MyTeam.Models.Domain.MemberTeam", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("MemberId");

                    b.Property<Guid>("TeamId");

                    b.HasKey("Id");

                    b.HasIndex("MemberId");

                    b.HasIndex("TeamId");

                    b.ToTable("MemberTeam");
                });

            modelBuilder.Entity("MyTeam.Models.Domain.Payment", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("Amount");

                    b.Property<Guid>("ClubId");

                    b.Property<string>("Comment");

                    b.Property<Guid>("MemberId");

                    b.Property<DateTime>("TimeStamp");

                    b.HasKey("Id");

                    b.HasIndex("ClubId");

                    b.HasIndex("MemberId");

                    b.ToTable("Payments");
                });

            modelBuilder.Entity("MyTeam.Models.Domain.PaymentInformation", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("ClubId");

                    b.Property<string>("Info");

                    b.HasKey("Id");

                    b.ToTable("PaymentInformation");
                });

            modelBuilder.Entity("MyTeam.Models.Domain.RemedyRate", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("ClubId");

                    b.Property<string>("Description");

                    b.Property<bool>("IsDeleted");

                    b.Property<string>("Name");

                    b.Property<int>("Rate");

                    b.HasKey("Id");

                    b.ToTable("RemedyRates");
                });

            modelBuilder.Entity("MyTeam.Models.Domain.Season", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("AutoUpdateFixtures");

                    b.Property<bool>("AutoUpdateTable");

                    b.Property<DateTime>("EndDate");

                    b.Property<string>("FixturesSourceUrl");

                    b.Property<DateTime?>("FixturesUpdated");

                    b.Property<string>("Name");

                    b.Property<DateTime>("StartDate");

                    b.Property<string>("TableJson");

                    b.Property<string>("TableSourceUrl");

                    b.Property<string>("TableString");

                    b.Property<DateTime>("TableUpdated");

                    b.Property<Guid>("TeamId");

                    b.HasKey("Id");

                    b.HasIndex("TeamId");

                    b.ToTable("Season");
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

                    b.HasIndex("ClubId");

                    b.ToTable("Team");
                });

            modelBuilder.Entity("MyTeam.Models.Domain.Game", b =>
                {
                    b.HasBaseType("MyTeam.Models.Domain.Event");

                    b.Property<int?>("AwayScore");

                    b.Property<string>("GamePlan");

                    b.Property<string>("GamePlanState");

                    b.Property<int?>("HomeScore");

                    b.Property<Guid>("TeamId");

                    b.HasIndex("TeamId");

                    b.ToTable("Game");

                    b.HasDiscriminator().HasValue("Game");
                });

            modelBuilder.Entity("MyTeam.Models.Domain.Player", b =>
                {
                    b.HasBaseType("MyTeam.Models.Domain.Member");

                    b.Property<int>("AssistCount");

                    b.Property<int>("GameCount");

                    b.Property<int>("GoalCount");

                    b.Property<string>("PositionsString");

                    b.ToTable("Player");

                    b.HasDiscriminator().HasValue("Player");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("MyTeam.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("MyTeam.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("MyTeam.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("MyTeam.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("MyTeam.Models.Domain.Article", b =>
                {
                    b.HasOne("MyTeam.Models.Domain.Member", "Author")
                        .WithMany("Articles")
                        .HasForeignKey("AuthorId");

                    b.HasOne("MyTeam.Models.Domain.Club", "Club")
                        .WithMany()
                        .HasForeignKey("ClubId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("MyTeam.Models.Domain.Game", "Game")
                        .WithOne("Report")
                        .HasForeignKey("MyTeam.Models.Domain.Article", "GameId");
                });

            modelBuilder.Entity("MyTeam.Models.Domain.Comment", b =>
                {
                    b.HasOne("MyTeam.Models.Domain.Article", "Article")
                        .WithMany("Comments")
                        .HasForeignKey("ArticleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("MyTeam.Models.Domain.Member", "Member")
                        .WithMany("Comments")
                        .HasForeignKey("MemberId");
                });

            modelBuilder.Entity("MyTeam.Models.Domain.Event", b =>
                {
                    b.HasOne("MyTeam.Models.Domain.Club", "Club")
                        .WithMany("Events")
                        .HasForeignKey("ClubId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("MyTeam.Models.Domain.EventAttendance", b =>
                {
                    b.HasOne("MyTeam.Models.Domain.Event", "Event")
                        .WithMany("Attendees")
                        .HasForeignKey("EventId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("MyTeam.Models.Domain.Member", "Member")
                        .WithMany("EventAttendances")
                        .HasForeignKey("MemberId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("MyTeam.Models.Domain.EventTeam", b =>
                {
                    b.HasOne("MyTeam.Models.Domain.Event", "Event")
                        .WithMany("EventTeams")
                        .HasForeignKey("EventId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("MyTeam.Models.Domain.Team", "Team")
                        .WithMany("EventTeams")
                        .HasForeignKey("TeamId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("MyTeam.Models.Domain.Fine", b =>
                {
                    b.HasOne("MyTeam.Models.Domain.Member", "Member")
                        .WithMany()
                        .HasForeignKey("MemberId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("MyTeam.Models.Domain.RemedyRate", "Rate")
                        .WithMany("Fines")
                        .HasForeignKey("RemedyRateId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("MyTeam.Models.Domain.GameEvent", b =>
                {
                    b.HasOne("MyTeam.Models.Domain.Player", "AssistedBy")
                        .WithMany("Assists")
                        .HasForeignKey("AssistedById")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("MyTeam.Models.Domain.Game", "Game")
                        .WithMany("GameEvents")
                        .HasForeignKey("GameId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("MyTeam.Models.Domain.Player", "Player")
                        .WithMany("GameEvents")
                        .HasForeignKey("PlayerId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("MyTeam.Models.Domain.Member", b =>
                {
                    b.HasOne("MyTeam.Models.Domain.Club")
                        .WithMany("Members")
                        .HasForeignKey("ClubId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("MyTeam.Models.Domain.MemberTeam", b =>
                {
                    b.HasOne("MyTeam.Models.Domain.Member", "Member")
                        .WithMany("MemberTeams")
                        .HasForeignKey("MemberId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("MyTeam.Models.Domain.Team", "Team")
                        .WithMany("MemberTeams")
                        .HasForeignKey("TeamId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("MyTeam.Models.Domain.Payment", b =>
                {
                    b.HasOne("MyTeam.Models.Domain.Club", "Club")
                        .WithMany()
                        .HasForeignKey("ClubId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("MyTeam.Models.Domain.Member", "Member")
                        .WithMany("Payments")
                        .HasForeignKey("MemberId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("MyTeam.Models.Domain.Season", b =>
                {
                    b.HasOne("MyTeam.Models.Domain.Team", "Team")
                        .WithMany("Seasons")
                        .HasForeignKey("TeamId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("MyTeam.Models.Domain.Team", b =>
                {
                    b.HasOne("MyTeam.Models.Domain.Club", "Club")
                        .WithMany("Teams")
                        .HasForeignKey("ClubId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("MyTeam.Models.Domain.Game", b =>
                {
                    b.HasOne("MyTeam.Models.Domain.Team", "Team")
                        .WithMany("Games")
                        .HasForeignKey("TeamId")
                        .OnDelete(DeleteBehavior.Restrict);
                });
#pragma warning restore 612, 618
        }
    }
}

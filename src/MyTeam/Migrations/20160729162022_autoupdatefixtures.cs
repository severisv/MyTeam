using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MyTeam.Migrations
{
    public partial class autoupdatefixtures : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AutoUpdateFixtures",
                table: "Season",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "FixturesSourceUrl",
                table: "Season",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AutoUpdateFixtures",
                table: "Season");

            migrationBuilder.DropColumn(
                name: "FixturesSourceUrl",
                table: "Season");
        }
    }
}

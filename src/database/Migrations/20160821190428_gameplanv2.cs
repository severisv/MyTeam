using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MyTeam.Migrations
{
    public partial class gameplanv2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GamePlan",
                table: "Event",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "GamePlanIsPublished",
                table: "Event",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GamePlan",
                table: "Event");

            migrationBuilder.DropColumn(
                name: "GamePlanIsPublished",
                table: "Event");
        }
    }
}

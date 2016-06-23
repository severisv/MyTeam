using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MyTeam.Migrations
{
    public partial class fine_merge_rates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExtraRate",
                table: "Fines");

            migrationBuilder.DropColumn(
                name: "StandardRate",
                table: "Fines");

            migrationBuilder.AddColumn<int>(
                name: "Amount",
                table: "Fines",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Amount",
                table: "Fines");

            migrationBuilder.AddColumn<int>(
                name: "ExtraRate",
                table: "Fines",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StandardRate",
                table: "Fines",
                nullable: true);
        }
    }
}

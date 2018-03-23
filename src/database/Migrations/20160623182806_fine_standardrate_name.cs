using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MyTeam.Migrations
{
    public partial class fine_standardrate_name : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RateName",
                table: "Fines",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StandardRate",
                table: "Fines",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RateName",
                table: "Fines");

            migrationBuilder.DropColumn(
                name: "StandardRate",
                table: "Fines");
        }
    }
}

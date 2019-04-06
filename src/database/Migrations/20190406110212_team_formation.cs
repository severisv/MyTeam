using Microsoft.EntityFrameworkCore.Migrations;

namespace MyTeam.Migrations
{
    public partial class team_formation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Formation",
                table: "Team",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Formation",
                table: "Team");
        }
    }
}

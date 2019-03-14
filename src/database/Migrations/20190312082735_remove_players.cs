using Microsoft.EntityFrameworkCore.Migrations;

namespace MyTeam.Migrations
{
    public partial class remove_players : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "Member");

            migrationBuilder.DropColumn(
                name: "AssistCount",
                table: "Member");

            migrationBuilder.DropColumn(
                name: "GameCount",
                table: "Member");

            migrationBuilder.DropColumn(
                name: "GoalCount",
                table: "Member");

            migrationBuilder.AlterColumn<bool>(
                name: "IsAttending",
                table: "EventAttendance",
                nullable: true,
                oldClrType: typeof(bool));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "Member",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "AssistCount",
                table: "Member",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GameCount",
                table: "Member",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GoalCount",
                table: "Member",
                nullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsAttending",
                table: "EventAttendance",
                nullable: false,
                oldClrType: typeof(bool),
                oldNullable: true);
        }
    }
}

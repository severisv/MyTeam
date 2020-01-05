using Microsoft.EntityFrameworkCore.Migrations;

namespace MyTeam.Migrations
{
    public partial class create_indexez : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Article_AuthorId",
                table: "Article",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Article_ClubId",
                table: "Article",
                column: "ClubId");

            migrationBuilder.CreateIndex(
                name: "IX_Article_GameId",
                table: "Article",
                column: "GameId",
                filter: "[GameId] IS NOT NULL");
           
            migrationBuilder.CreateIndex(
                name: "IX_Event_ClubId",
                table: "Event",
                column: "ClubId");

            migrationBuilder.CreateIndex(
                name: "IX_Event_TeamId",
                table: "Event",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_EventAttendance_EventId",
                table: "EventAttendance",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_EventAttendance_MemberId",
                table: "EventAttendance",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_EventTeam_EventId",
                table: "EventTeam",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_EventTeam_TeamId",
                table: "EventTeam",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_GameEvent_AssistedById",
                table: "GameEvent",
                column: "AssistedById");

            migrationBuilder.CreateIndex(
                name: "IX_GameEvent_GameId",
                table: "GameEvent",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_GameEvent_PlayerId",
                table: "GameEvent",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_Member_ClubId",
                table: "Member",
                column: "ClubId");

            migrationBuilder.CreateIndex(
                name: "IX_MemberTeam_MemberId",
                table: "MemberTeam",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_MemberTeam_TeamId",
                table: "MemberTeam",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_Season_TeamId",
                table: "Season",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_Team_ClubId",
                table: "Team",
                column: "ClubId");

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}

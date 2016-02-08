using System;
using System.Text.RegularExpressions;
using MyTeam.Models.Enums;

namespace MyTeam.Settings
{
    public class Icons
    {
        public const string Attendance = "fa fa-check-square-o";
        public const string Coach = "flaticon-football50";
        public const string Signup = "fa fa-calendar";
        public const string Previous = "fa fa-history";
        public const string Upcoming = "fa fa-calendar-o";
        public const string SquadList = "fa fa-users";

        public static string GameType(GameType? gameType)
        {
            switch (gameType)
            {
                case Models.Enums.GameType.Seriekamp:
                    return "fa fa-trophy";
                case Models.Enums.GameType.Cupkamp:
                    return "flaticon-football42";
                case Models.Enums.GameType.Treningskamp:
                    return "icon-handshake";
                default:
                    return string.Empty;
            }
        }
    }
}
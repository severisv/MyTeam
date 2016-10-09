
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
        public const string Fine = "fa fa-money";
        public static string Payment = "fa fa-list";

        public static string GameType(GameType? gameType)
        {
            switch (gameType)
            {
                case Models.Enums.GameType.Seriekamp:
                    return "fa fa-trophy";
                case Models.Enums.GameType.Norgesmesterskapet:
                    return "flaticon-football42";
                case Models.Enums.GameType.Treningskamp:
                    return "icon-handshake";
                case Models.Enums.GameType.Kretsmesterskapet:
                    return "flaticon-football33";
                case Models.Enums.GameType.ObosCup:
                    return "flaticon-trophy4";
                default:
                    return string.Empty;
            }
        }
    }
}
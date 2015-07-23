using System;
using MyTeam.Models.Domain;

namespace MyTeam.Resources
{
    public class Res
    {
        public static string Home = "Nyheter";
        public static string Players = "Spillere";

        public static string Games = "Kamper";
        public static string Table = "Tabell";
        public const string GameCount = "Kamper";
        public const string GoalCount = "Mål";
        public const string AssistCount = "Assists";
        public const string Positions = "Posisjon";
        public const string BirthDate = "Født";
        public const string StartDate = "Signerte for";
        public const string Email = "E-post";
        public const string Phone = "Telefon";
        public const string Active = "Aktive";
        public static string ActivePlayers => string.Format("{0} {1}", Active, Players);
        public const string Retired = "Pensjonerte";
        public static string RetiredPlayers => string.Format("{0} {1}", Retired, Players);
        public const string HallOfFame = "Hall of Fame";
        public const string Submenu = "Undermeny";


        /* Training */
        public static string Trainings = "Treninger";
        public static string UpcomingTrainings => $"Kommende {Trainings.ToLowerInvariant()}";
        public static string PreviousTrainings => $"Tidligere {Trainings.ToLowerInvariant()}";



        public static string PlayersOfType(PlayerStatus status)
        {
            if (status == PlayerStatus.Aktiv) return string.Format("{0}", ActivePlayers);
            if (status == PlayerStatus.Pensjonert) return string.Format("{0}", HallOfFame);
            return Players;
        }
    }
}
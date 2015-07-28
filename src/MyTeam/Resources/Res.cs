using System;
using MyTeam.Models.Domain;
using MyTeam.Models.Enums;

namespace MyTeam.Resources
{
    public class Res
    {
        public const string Home = "Nyheter";
        public const string Players = "Spillere";
        public const string Event = "Hendelse";
        public const string Attending = "Kommer";
        public const string IsAttending = "Stiller";
        public const string IsNotAttending = "Kan ikke";

        public const string Games = "Kamper";
        public const string Table = "Tabell";
        public const string GameCount = "Kamper";
        public const string GoalCount = "Mål";
        public const string AssistCount = "Assists";
        public const string Positions = "Posisjon";
        public const string BirthDate = "Født";
        public const string StartDate = "Signerte for";
        public const string Email = "E-post";
        public const string Phone = "Telefon";
        public const string Active = "Aktive";
        public static string ActivePlayers => $"{Active} {Players}";
        public const string Retired = "Pensjonerte";
        public static string RetiredPlayers => $"{Retired} {Players}";
        public const string HallOfFame = "Hall of Fame";
        public const string Submenu = "Undermeny";
        public static string Upcoming = "Kommende";
        public static string Previous = "Tidligere";
        public static string Pluralise = "er";


        /* Training */
        public static string Trainings = "Treninger";




        public static string PlayersOfType(PlayerStatus status)
        {
            if (status == PlayerStatus.Aktiv) return string.Format("{0}", ActivePlayers);
            if (status == PlayerStatus.Pensjonert) return string.Format("{0}", HallOfFame);
            return Players;
        }
    }
}
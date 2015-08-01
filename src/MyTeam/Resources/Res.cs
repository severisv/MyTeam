using System;
using MyTeam.Models.Domain;
using MyTeam.Models.Enums;

namespace MyTeam.Resources
{
    public class Res
    {

        public const string Actions = "Handlinger";
        public const string Attending = "Kommer";
        public const string AssistCount = "Assists";
        public const string Active = "Aktive";
        public static string ActivePlayers => $"{Active} {Players}";

        public const string BirthDate = "Født";


        public const string ConfirmDelete = "Er du sikker på at du vil slette?";
        public const string Create = "Opprett";
        public static string CreateEvent => $"{Create} { Event.ToLower()}";
        public const string CoachMenu = "Trenermeny";
        public const string Description = "Beskrivelse";
        public const string Date = "Dato";
        public const string Delete = "Slett";
        public const string Deleted = "Slettet";
        public const string Edit = "Rediger";
        public const string Error = "Det oppstod en feil";
        public const string Event = "Hendelse";
        public const string FieldRequired = "Feltet er obligatorisk";

        public const string Home = "Nyheter";


        public const string IsAttending = "Stiller";
        public const string IsNotAttending = "Kan ikke";
        public const string InvalidInput = "Ugyldig input";

        public const string Games = "Kamper";
        public const string GameCount = "Kamper";
        public const string GoalCount = "Mål";
        public const string Location = "Sted";
        public const string HallOfFame = "Hall of Fame";
        public const string Mandatory = "Obligatorisk oppmøte";
        public const string MandatoryExplaination = "Obligatorisk oppmøte betyr at treningen teller med på oppmøtestatistikken";
        public const string NotFound = "Siden finnes ikke";


        public const string Table = "Tabell";

        public const string Positions = "Posisjon";
        public const string StartDate = "Signerte for";
        public const string Email = "E-post";
        public const string Phone = "Telefon";
        public const string Recurring = "Ukentlig";
        public const string RecurringExplaination = "Hvis man haker av for at treningen skal være ukentlig vil det opprettes treninger hver uke på samme tidspunkt frem til datoen man spesifiserer";
        public const string Retired = "Pensjonerte";
        public static string RetiredPlayers => $"{Retired} {Players}";
        public const string Save = "Lagre";

        public static string SignoffClosed = $"Man kan ikke melde seg av arrangementer senere enn {Settings.Config.AllowedSignoffHours} timer før de begynner";
        public static string SignupNotAllowed = $"Man kan ikke melde seg på arrangementer lengre enn {Settings.Config.AllowedSignupDays} dager frem i tid";
        public const string Submenu = "Undermeny";
        public const string Upcoming = "Kommende";
        public const string Previous = "Tidligere";
        public const string Pluralise = "er";
        public const string Players = "Spillere";

        public const string Time = "Klokkeslett";
        public const string Trainings = "Treninger";
        public const string ToDate = "Til dato";
        public const string TrainingCreationTooFarAheadInTime = "Kan ikke opprette treninger så langt frem i tid";
        public const string Unauthorized = "Ingen tilgang";




        public static string PlayersOfType(PlayerStatus status)
        {
            if (status == PlayerStatus.Aktiv) return string.Format("{0}", ActivePlayers);
            if (status == PlayerStatus.Pensjonert) return string.Format("{0}", HallOfFame);
            return Players;
        }
    }
}
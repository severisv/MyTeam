using System;
using MyTeam.Models.Domain;
using MyTeam.Models.Enums;

namespace MyTeam.Resources
{
    public class Res
    {

        public const string Attending = "Kommer";
        public const string Attendance = "Oppmøte";
        public const string AssistCount = "Assists";
        public const string Add = "Legg til";
        public const string Added = "Lagt til";
        public const string AdminPage = "Admin";
        public const string Active = "Aktive";
        public static string ActivePlayers => $"{Active} {Players}";

        public const string BirthDate = "Født";


        public const string ConfirmDelete = "Er du sikker på at du vil slette?";
        public const string Create = "Opprett";
        public static string CreateEvent => $"{Create} { Event.ToLower()}";
        public const string CoachMenu = "Adminmeny";
        public const string Description = "Beskrivelse";
        public const string Date = "Dato";
        public const string Delete = "Slett";
        public const string Deleted = "Slettet";

        public const string Email = "E-post";
        public const string Edit = "Rediger";
        public const string Error = "Det oppstod en feil";
        public const string Event = "Arrangement";
        public const string FieldRequired = "Feltet er obligatorisk";
        public const string Goal = "Mål";
        public const string Games = "Kamper";
        public const string GameCount = "Kamper";
        public const string GoalCount = "Mål";
        public const string About = "Om klubben";
        public const string HomeGround = "Hjemme";
        public const string Internal = "Intern";
        public const string IsAttending = "Stiller";
        public const string IsAlready = "Er allerede";
        public const string IsRequired = "Er obligatorisk";
        public const string IsNotAttending = "Kan ikke";
        public const string InvalidInput = "Ugyldig input";
        public const string Logout = "Logg ut";
        public const string Login = "Logg inn";

        public const string Location = "Sted";
        public const string HallOfFame = "Hall of Fame";
        public const string Headline = "Overskrift";
        public const string ManagePlayers = "Administrer spillere";
        public const string Mandatory = "Tellende oppmøte";
        public const string MandatoryExplaination = "Tellende oppmøte betyr at treningen teller med på oppmøtestatistikken";
        public const string Name = "Navn";


        public const string Table = "Tabell";

        public const string Positions = "Posisjon";
        public const string StartDate = "Signerte for klubben";
        public const string Opponent = "Motstander";
        public const string Phone = "Telefon";
        public const string Player = "Spiller";
        public const string RedCard = "Rødt kort";
        public const string Register = "Registrer";
        public const string Recurring = "Ukentlig";
        public const string RecurringExplaination = "Hvis man haker av for at treningen skal være ukentlig vil det opprettes en trening for hver uke på samme tidspunkt frem til datoen som er spesifisert";
        public const string Type = "Type";

        public static string TeamSelection = "Laguttak";
        public const string Team = "Lag";

        public static string Stats = "Statistikk";

        public const string Saved = "Lagret";
        public const string Season = "Sesong";
        public const string Signup = "Påmelding";

        public static string SignoffClosed = $"Man kan ikke melde seg av arrangementer senere enn {Settings.Config.AllowedSignoffHours} timer før de begynner";
        public const string Submenu = "Undermeny";
        public const string Upcoming = "Kommende";
        public const string Previous = "Tidligere";
        public const string Players = "Spillere";
        public const string Save = "Lagre";
        public const string SquadList = "Lagliste";
       


        public const string Time = "Klokkeslett";
        public const string ToDate = "Til dato";
        public const string TrainingCreationTooFarAheadInTime = "Kan ikke opprette treninger så langt frem i tid";
        public const string Unauthorized = "Ingen tilgang";
        public const string Voluntary = "Frivillig";
        public const string YellowCard = "Gult kort";



        public static string PlayersOfType(PlayerStatus status)
        {
            if (status == PlayerStatus.Aktiv) return string.Format("{0}", ActivePlayers);
            if (status == PlayerStatus.Veteran) return string.Format("{0}", HallOfFame);
            return Players;
        }
    }
}

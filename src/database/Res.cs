using System;
using MyTeam.Models.Domain;
using MyTeam.Models.Enums;

namespace MyTeam.Resources
{
    public class Res
    {

        public const string Admin = "Admin";
        public const string Actions = "Handlinger";
        public const string Attending = "Kommer";
        public const string Attendance = "Oppmøte";
        public const string Assist = "Assist";
        public const string AssistCount = "Assists";
        public const string Add = "Legg til";
        public const string Added = "Lagt til";
        public const string AdminPage = "Admin";
        public const string Active = "Aktive";
        public static string ActivePlayers => $"{Active} {Players}";
        public static string Archive => "Arkiv";

        public const string BirthDate = "Født";


        public const string ConfirmDelete = "Er du sikker på at du vil slette?";
        public const string Create = "Opprett";
        public const string Created = "Opprettet";
        public static string CreateEvent => $"{Create} { Event.ToLower()}";
        public const string CoachMenu = "Adminmeny";
        public const string Description = "Beskrivelse";
        public const string Date = "Dato";
        public const string Delete = "Slett";
        public const string Deleted = "Slettet";
        public const string DoesNotExistAny = "Det finnes ingen";
        public const string Draw = "Uavgjort";

        public const string Email = "E-post";
        public const string Edit = "Rediger";
        public const string Error = "Det oppstod en feil";
        public const string Event = "Arrangement";
        public const string FieldRequired = "Feltet er obligatorisk";
        public const string Goal = "Mål";
        public const string GoalDifference = "Målforskjell";
        public const string Game = "Kamp";
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
        public const string LastUpdated = "Sist oppdatert";
        public const string Loss = "Tap";
        public const string Logout = "Logg ut";
        public const string Login = "Logg inn";

        public const string Location = "Sted";
        public const string HallOfFame = "Hall of Fame";
        public const string Headline = "Overskrift";
        public const string Manage = "Administrer";
        public const string ManagePlayers = "Administrer spillere";
        public const string Mandatory = "Tellende oppmøte";
        public const string MandatoryExplaination = "Tellende oppmøte betyr at treningen teller med på oppmøtestatistikken";
        public const string Name = "Navn";
        public const string NoShows = "Ikke møtt";


        public const string Table = "Tabell";

        public const string Positions = "Posisjon";
        public const string StartDate = "Signerte for klubben";
        public const string Opponent = "Motstander";
        public const string Phone = "Telefon";
        public const string Player = "Spiller";
        public const string Points = "Poeng";
        public const string RedCard = "Rødt kort";
        public const string Register = "Registrer";
        public const string Recurring = "Ukentlig";
        public const string RecurringExplaination = "Hvis man haker av for at treningen skal være ukentlig vil det opprettes en trening for hver uke på samme tidspunkt frem til datoen som er spesifisert";
        public const string Retired = "Pensjonerte";
        public static string RetiredPlayers => $"{Retired} {Players}";

        public const string Type = "Type";

        public static string TeamSelection = "Laguttak";
        public const string Team = "Lag";

        public static string Stats = "Statistikk";

        public const string Saved = "Lagret";
        public const string Season = "Sesong";
        public const string Signup = "Påmelding";

        public static string SignoffClosed = $"Man kan ikke melde seg av arrangementer senere enn {Settings.Config.AllowedSignoffHours} timer før de begynner";
        public static string SignupNotAllowed = $"Man kan ikke melde seg på arrangementer lengre enn {Settings.Config.AllowedSignupDays} dager frem i tid";
        public const string Submenu = "Undermeny";
        public const string Upcoming = "Kommende";
        public const string Password = "Passord";
        public const string Previous = "Tidligere";
        public const string Pluralise = "er";
        public const string Players = "Spillere";
        public const string Position = "Pos";
        public const string Save = "Lagre";
        public const string SquadList = "Lagliste";
        public const string SubmitTable = "Lim inn tabell på formatet \n" +
                                          "Plass   Lag       Kamper   [  Hjemme  ]    [  Borte  ]     [   Total    ]  Diff  Poeng    \neks:\n\n" +
"1	Oslojuvelene	11	4	1	0	24 - 5	6	0	0	19 - 5	10	1	0	43 - 10	33	31 \n" +
"2	Fortuna	        11	4	0	2	18 - 8	4	0	1	19 - 5	8	0	3	37 - 13	24	24         \n" +
"3	Grüner	        11	3	1	1	16 - 9	3	2	1	12 - 9	6	3	2	28 - 18	10	21           \n" +
"4	Asker 2	        11	3	0	2	16 - 15	3	1	2	13 - 8	6	1	4	29 - 23	6	19           \n" +
"5	Høybr/Stovn    11	3	1	1	13 - 10	2	2	2	10 - 11	5	3	3	23 - 21	2	18       \n" +
"6	Heggedal         11	2	3	2	16 - 17	3	0	1	7 - 4	5	3	3	23 - 21	2	18       \n" +
"7	Vollen	        11	2	0	2	9 - 3	        3	2	2	25 - 26	5	2	4	34 - 29	5	17           \n" +
"8	Oldenborg 2    11	1	1	3	6 - 12	2	0	4	16 - 24	3	1	7	22 - 36	-14	10       \n" +
"9	Wam-Kam       10	0	0	6	5 - 27	3	0	1	8 - 8	3	0	7	13 - 35	-22	9            \n" +
"10	Fossum	        11	2	0	4	11 - 16	0	0	5	3 - 19	2	0	9	14 - 35	-21	6            \n" +
"11	Vestli	        11	0	0	6	4 - 13	0	0	5	3 - 19	0	0	11	7 - 32	-25	0            \n" +
"12	Bøkeby	        0	0	0	0	0 - 0	      0	0	0	0 - 0	0	0	0	0 - 0	0	0            \n";


        public const string SubmitGames = "Lim inn terminliste på formatet \n" +
                                          "Dato \t  Dag   \t    Tid  \t Hjemmelag \t   3 : 1  \t  Bortelag \t    Sted \neks:\n\n" +
"14.04.2015	\ttirsdag	\t20:15\t	Wam-Kam	\t0 : 2	\tHøybr/Stovn \tJordal kunstgress \n" +
"21.04.2015	\ttirsdag	\t20:15\t	Oldenborg \t2	1 : 2	\tWam-Kam Tørteberg kunstgress \n" +
"28.04.2015	\ttirsdag	\t20:15\t	Wam-Kam	\t0 : 3     \tHeggedal \tJordal kunstgress \n" +
"05.05.2015	\ttirsdag	\t20:15\t	Fossum	          \t3 : 5	\tWam-Kam \tFossum kunstgress \n" +
"12.05.2015	\ttirsdag	\t20:15\t	Wam-Kam	\t1 : 5	\tGrüner \tJordal kunstgress \n" +
"19.05.2015	\ttirsdag	\t20:15\t	Wam-Kam	\t1 : 5	\tFortuna \tJordal kunstgress \n" +
"26.05.2015	\ttirsdag	\t20:15\t	Vestli	         \t0 : 1	\tWam-Kam \tVestli kunstgress \n" +
"02.06.2015	\ttirsdag	\t20:15\t	Wam-Kam	\t0 : 7	\tAsker 2	\tJordal kunstgress\n" +
"09.06.2015	\ttirsdag	\t20:15\t	Oslojuvelene	\t4 : 0	\tWam-Kam \tBjølsen kunstgress\n" +
"16.06.2015	\ttirsdag	\t20:15\t	Wam-Kam	\t3 : 5	\tVollen \tJordal kunstgress \n" +
"04.08.2015	\ttirsdag	\t20:15\t	Høybr/Stovn	\t3 : 2	\tWam-Kam \tHøybråten kunstgress";


        public const string Time = "Klokkeslett";
        public const string Training = "Trening";
        public const string Trainings = "Treninger";
        public const string ToDate = "Til dato";
        public const string TrainingCreationTooFarAheadInTime = "Kan ikke opprette treninger så langt frem i tid";
        public const string Unauthorized = "Ingen tilgang";
        public const string Update = "Oppdater";
        public const string User = "Bruker";
        public const string Voluntary = "Frivillig";
        public const string Win = "Seier";
        public const string YellowCard = "Gult kort";



        public static string PlayersOfType(PlayerStatus status)
        {
            if (status == PlayerStatus.Aktiv) return string.Format("{0}", ActivePlayers);
            if (status == PlayerStatus.Veteran) return string.Format("{0}", HallOfFame);
            return Players;
        }
    }
}

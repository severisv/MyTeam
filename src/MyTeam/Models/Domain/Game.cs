namespace MyTeam.Models.Domain
{
    public class Game : Event
    {
        public string Opponent { get; set; }
        public virtual Article Report { get; set; }

    }
}
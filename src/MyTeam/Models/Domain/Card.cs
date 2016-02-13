using MyTeam.Models.Enums;

namespace MyTeam.Models.Domain
{
    public class Card : GameEvent
    {
        public CardType Type { get; set; }
    }
}
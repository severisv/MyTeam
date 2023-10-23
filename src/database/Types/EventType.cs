namespace MyTeam.Models.Enums
{
    public enum EventType
    {
        Alle,
        Trening,
        Kamp,
        Diverse
    }

    public static class EventTypeMappings
    {
        public static int ToInt(this EventType type) => (int)type;
        public static EventType FromInt(this int type) => (EventType)type;
    }
}
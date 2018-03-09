using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MyTeam.Models.Enums
{
   [JsonConverter(typeof(StringEnumConverter))] 
   public enum PlayerStatus
    {
        Aktiv,
        Inaktiv,
        Veteran,
        Trener,
        Sluttet
    }
}

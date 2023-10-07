using System.ComponentModel.DataAnnotations;

namespace MyTeam.Models.Enums
{
    public enum GameEventType
    {
        [Display(Name = "Mål")]
        Goal = 0,
        [Display(Name = "Gult kort")]
        YellowCard = 1,
        [Display(Name = "Rødt kort")]
        RedCard = 2
    }
}
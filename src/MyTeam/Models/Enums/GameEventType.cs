using System.ComponentModel.DataAnnotations;
using MyTeam.Resources;

namespace MyTeam.Models.Enums
{
    public enum GameEventType
    {
        [Display(Name = Res.Goal)]
        Goal,
        [Display(Name = Res.Assist)]
        Assist,
        [Display(Name = Res.YellowCard)]
        YellowCard,
        [Display(Name = Res.RedCard)]
        RedCard
    }
}
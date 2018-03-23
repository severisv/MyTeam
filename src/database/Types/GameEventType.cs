using System.ComponentModel.DataAnnotations;
using MyTeam.Resources;

namespace MyTeam.Models.Enums
{
    public enum GameEventType
    {
        [Display(Name = Res.Goal)]
        Goal = 0,
        [Display(Name = Res.YellowCard)]
        YellowCard = 1,
        [Display(Name = Res.RedCard)]
        RedCard = 2
    }
}
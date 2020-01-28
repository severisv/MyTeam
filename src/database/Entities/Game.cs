using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyTeam.Models.Enums
{
    public enum GameType
    {
        Treningskamp,
        Seriekamp,
        Norgesmesterskapet,
        Kretsmesterskapet,
        [Display(Name = "OBOS Cup")]
        ObosCup
    }
}

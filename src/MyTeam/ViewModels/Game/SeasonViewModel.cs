using System;

namespace MyTeam.ViewModels.Game
{
    public class SeasonViewModel
    {
        public int Year { get; set; }
        public string DisplayName => Year.ToString();
    }
}
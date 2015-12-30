using System;
using System.Collections.Generic;

namespace MyTeam.ViewModels.Game
{
    public class GameViewModel
    {
        public DateTime DateTime { get; set; }
        public string Opponent { get; set; }
        public IEnumerable<string> Teams { get; set; }
        public int Goals { get; set; }
    }
}
using System.Collections.Generic;
using MyTeam.Models.Dto;

namespace MyTeam.ViewModels.Game
{
    public class ShowGameViewModel
    {
        public GameViewModel Game { get; set; }
        public IEnumerable<PlayerDto> Squad { get; set; } 
        
    }
}
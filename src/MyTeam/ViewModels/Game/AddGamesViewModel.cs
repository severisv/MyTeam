using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using MyTeam.Models.Enums;
using MyTeam.Resources;
using MyTeam.ViewModels.Table;

namespace MyTeam.ViewModels.Game
{
    public class AddGamesViewModel : IValidatableObject
    {
        [Display(Name = Res.Team)]
        public IList<TeamViewModel> Teams { get; set; }
        public string GamesString { get; set; }
        [Required]
        [Display(Name = Res.Team)]
        public Guid TeamId { get; set; }
        public GameType GameType { get; set; }
        public string TeamName => Teams?.SingleOrDefault(t => t.Id == TeamId)?.Name;

        public virtual GamesList Games
        {
            get
            {
                try
                {
                    return new GamesList(TeamId, TeamName, GamesString, GameType);
                }
                catch
                {
                    return null;
                }
            }
        }

        public IEnumerable<GameType> GameTypes => Enum.GetValues(typeof(GameType)).Cast<GameType>();



        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var result = new List<ValidationResult>();

            if (Games == null)
            {
                result.Add(new ValidationResult(Res.InvalidInput, new[] { nameof(GamesString) }));
            }
            return result;
        }
    }
}

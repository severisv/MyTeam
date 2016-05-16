using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using MyTeam.Resources;

namespace MyTeam.ViewModels.Table
{
    public class CreateTableViewModel : UpdateSeasonViewModel, IValidatableObject
    {
     
        public virtual ParsedTable Table
        {
            get
            {
                try
                {
                    return new ParsedTable(SeasonId, TableString);
                }
                catch
                {
                    return null;
                }
            }
        }



        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var result = new List<ValidationResult>();

            if (Table == null)
            {
                result.Add(new ValidationResult(Res.InvalidInput, new[] { nameof(TableString) }));
            }
            return result;
        }

        public string ConvertTableString()
        {
            return string.Join("|", Table.Lines.Select(JoinLine));
        }

      

        public string JoinLine(ParsedTableTeam tableteam)
        {
            return string.Join(";", new[]
           {
                tableteam.Position.ToString(),
                tableteam.Name,
                tableteam.Wins.ToString(),
                tableteam.Draws.ToString(),
                tableteam.Losses.ToString(),
                tableteam.GoalsFor.ToString(),
                tableteam.GoalsAgainst.ToString(),
                tableteam.Points.ToString()
            });
        }
    }
}

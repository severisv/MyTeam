using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using MyTeam.Models.Domain;
using MyTeam.Resources;

namespace MyTeam.ViewModels.Table
{
    public class CreateTableViewModel : IValidatableObject
    {

        public string TableString { get; set; }
        [Required]
        public Guid SeasonId { get; set; }

        public virtual Models.Domain.Table Table
        {
            get
            {
                try
                {
                    return new Models.Domain.Table(SeasonId, TableString);
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
    }
}

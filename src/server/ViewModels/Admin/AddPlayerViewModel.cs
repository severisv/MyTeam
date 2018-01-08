using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MyTeam.Resources;

namespace MyTeam.ViewModels.Admin
{
    public class AddPlayerViewModel : IValidatableObject
    {
        public string FacebookId { get; set; }

        [DataType(DataType.EmailAddress)]
        public string EmailAddress { get; set; }
        [Required]
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        [Required]
        public string LastName { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            if (string.IsNullOrWhiteSpace(FacebookId) && string.IsNullOrWhiteSpace(EmailAddress))
            {
                results.Add(new ValidationResult("E-postadresse er obligatorisk"));
            }

            return results;
        }
    }
}
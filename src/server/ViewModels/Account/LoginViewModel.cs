using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyTeam.ViewModels.Account
{
    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "E-post")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Passord")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Husk meg")]
        public bool RememberMe { get; set; }
    }
}

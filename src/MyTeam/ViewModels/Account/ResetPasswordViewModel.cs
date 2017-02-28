using System;
using System.ComponentModel.DataAnnotations;


namespace MyTeam.ViewModels.Account
{
    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "E-post")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "{0} må inneholde minst {2} tegn.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Passord")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Bekreft passord")]
        [Compare("Password", ErrorMessage = "Passordene stemmer ikke overens.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }
}

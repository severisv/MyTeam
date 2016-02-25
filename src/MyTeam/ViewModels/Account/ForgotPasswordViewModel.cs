using System.ComponentModel.DataAnnotations;


namespace MyTeam.ViewModels.Account
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "E-postadresse")]
        public string Email { get; set; }
    }
}

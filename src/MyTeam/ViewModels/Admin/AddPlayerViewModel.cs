using System.ComponentModel.DataAnnotations;

namespace MyTeam.ViewModels.Admin
{
    public class AddPlayerViewModel
    {
        [Required]
        public string FacebookId { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }

    }
}
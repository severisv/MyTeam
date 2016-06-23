using System;
using System.ComponentModel.DataAnnotations;
using MyTeam.Resources;
using MyTeam.Validation.Attributes;

namespace MyTeam.ViewModels.Fine
{
    public class FineViewModel
    {
        public Guid Id { get; set; }
        [RequiredNO]
        [Display(Name = Res.Name)]
        public string Name { get; set; }
        [Display(Name = Res.Name)]
        public string FirstName { get; set; }
        [Display(Name = Res.Name)]
        public string LastName { get; set; }
        public Guid MemberId { get; set; }
        public DateTime Issued { get; set; }
        public DateTime? PaidDate { get; set; }
        [RequiredNO]
        [Display(Name = "Sats")]
        [Range(1, 1000000, ErrorMessage = "Boten må være på mer enn 0")]

        public int Rate { get; set; }
        public bool IsPaid => PaidDate != null;

        public string Comment { get; set; }
        public string Description { get; set; }
        public string MemberImage { get; set; }
        public string FacebookId { get; set; }

    }
}
using System;
using System.ComponentModel.DataAnnotations;
using MyTeam.Resources;
using MyTeam.Validation.Attributes;

namespace MyTeam.ViewModels.Payment
{
    public class PaymentViewModel
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
        public DateTime TimeStamp { get; set; }
        [RequiredNO]
        [Display(Name = "Beløp")]
        [Range(1, 1000000, ErrorMessage = "Innbetalingen må være på mer enn 0")]

        public int Amount { get; set; }
        public string Comment { get; set; }
        public string MemberImage { get; set; }
        public string FacebookId { get; set; }

    }
}
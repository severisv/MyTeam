using System;

namespace MyTeam.Models.Domain
{
    public class PaymentInformation : Entity
    {
        public Guid ClubId { get; set; }
        public string Info { get; set; }
    }
}
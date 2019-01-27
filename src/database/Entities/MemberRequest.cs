using System;

namespace MyTeam.Models.Domain
{
    public class MemberRequest {

        public int Id { get; set; }
        public Guid ClubId { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string FacebookId { get; set; }
       
    }
}
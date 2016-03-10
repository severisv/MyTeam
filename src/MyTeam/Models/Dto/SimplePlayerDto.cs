using System;
using System.Collections.Generic;
using MyTeam.Models.Enums;

namespace MyTeam.Models.Dto
{
    public class SimplePlayerDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Name => $"{FirstName} {MiddleName} {LastName}";
        public string ShortName => $"{FirstName} {LastName}";
        public string ImageFull { get; set; }
        public string FacebookId { get; set; }
        public PlayerStatus Status { get; set; }
        public IEnumerable<Guid> TeamIds { get; set; }
    }
}
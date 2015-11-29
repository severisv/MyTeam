using System;
using System.ComponentModel.DataAnnotations;

namespace MyTeam.Models.Domain
{
    public class Game : Event
    {
        [Required]
        public string Opponent { get; set; }
        public Guid? ReportId { get; set; }
        public virtual Article Report { get; set; }

    }
}
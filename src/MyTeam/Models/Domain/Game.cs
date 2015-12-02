using System;
using System.ComponentModel.DataAnnotations;

namespace MyTeam.Models.Domain
{
    public class Game : Event
    {
 
        public Guid? ReportId { get; set; }
        public virtual Article Report { get; set; }

    }
}
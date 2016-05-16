using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MyTeam.ViewModels.Table;

namespace MyTeam.Models.Domain
{
    public class Season : Entity
    {
        [Required]
        public Guid TeamId { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        public string Name { get; set; }

        public DateTime TableUpdated { get; set; }

        [NotMapped]
        public Table Table => new Table(TableString);

        public string TableString { get; set; }

        public virtual Team Team { get; set; }
        public bool AutoUpdateTable { get; set; }
        public string TableSourceUrl { get; set; }

    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace MyTeam.Models.Domain
{
    public class Table : Entity
    {
        [Required]
        public Guid SeasonId { get; set; }
        [Required]
        public DateTime CreatedDate { get; set; }
        [NotMapped]
        public IList<TableTeam> Lines => ParseTable(TableString);
        [Required]
        public string TableString { get; set; }
        public virtual Season Season { get; set; }

        public Table()
        {
            
        }

        public Table(Guid seasonId, string table)
        {
            CreatedDate = DateTime.Now;
            SeasonId = seasonId;
            TableString = table;

            try
            {
                var parsedTable = ParseTable(TableString);
                if (!parsedTable.Any()) throw new ArgumentException("Invalid TableString, table does not contain any entries", nameof(TableString));

            }
            catch (Exception e)
            {
                throw new ArgumentException("Invalid TableString, could not create Table object", nameof(TableString),e);
            }
        }

        private IList<TableTeam> ParseTable(string tableString)
        {
            var table = tableString.Split('\n');

            return table.Select(line => new TableTeam(line)).Where(tableTeam => tableTeam.Position > -1).ToList();
        }
    }
}
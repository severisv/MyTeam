using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using MyTeam.Models.Domain;

namespace MyTeam.ViewModels.Table
{
    public class ParsedTable
    {
        [Required]
        public Guid SeasonId { get; set; }
        [Required]
        public DateTime CreatedDate { get; set; }
        [NotMapped]
        public IList<ParsedTableTeam> Lines => ParseTable(TableString);
        [Required]
        public string TableString { get; set; }
        public virtual Season Season { get; set; }

        public ParsedTable()
        {
            
        }

        public ParsedTable(Guid seasonId, string table)
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

        private IList<ParsedTableTeam> ParseTable(string tableString)
        {
            var table = tableString.Split('\n');

            return table.Select(line => new ParsedTableTeam(line)).Where(tableTeam => tableTeam.Position > -1).ToList();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;

namespace MyTeam.Models.Domain
{
    public class Table
    {

        public IList<TableTeam> Lines => ParseTable(TableString);
        public string TableString { get; set; }

        public Table(string table)
        {
            
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
            var table = tableString.Split('|');

            return table.Select(line => new TableTeam(line)).Where(tableTeam => tableTeam.Position > -1).ToList();
        }
    }
}
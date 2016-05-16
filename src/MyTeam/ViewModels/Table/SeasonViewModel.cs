using System;

namespace MyTeam.ViewModels.Table
{
    public class SeasonViewModel
    {
        public Guid Id { get; set; }
        public Guid TeamId { get; set; }
        public TeamViewModel Team { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string TableString { get; set; }
        public Models.Domain.Table Table => string.IsNullOrWhiteSpace(TableString) ? null : new Models.Domain.Table(Id, TableString);
        public string Name { get; set; }
        public string DisplayName => StartDate.Year == EndDate.Year ? $"{Name} - {EndDate.Year}" : $"{Name} - {StartDate.Year} / {EndDate.Year}";
        public string DisplayNameShort => StartDate.Year == EndDate.Year ? $"{EndDate.Year}" : $"{StartDate.Year} / {EndDate.Year}";

    }
}
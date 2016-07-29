using System;
using MyTeam.Models.Shared;

namespace MyTeam.ViewModels.Stats
{
    public class PlayerStats : IMember
    {
        public Guid Id { get; set; }
        public string FacebookId { get; set; }
        public int Goals { get; set; }
        public int Assists { get; set; }
        public int YellowCards { get; set; }
        public int RedCards { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string UrlName { get; set; }
        public string Image { get; set; }
        public int Games { get; set; }

      
    }
}
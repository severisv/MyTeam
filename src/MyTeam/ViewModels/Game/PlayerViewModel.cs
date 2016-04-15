using System;

namespace MyTeam.ViewModels.Game
{
    public class PlayerViewModel
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
    }
}
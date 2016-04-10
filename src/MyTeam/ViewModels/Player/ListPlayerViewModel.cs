using System;
using MyTeam.Models.Shared;

namespace MyTeam.ViewModels.Player
{
    public class ListPlayerViewModel : IMember
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string FacebookId { get; set; }
        public string Image { get; set; }
        public string Name => this.Name();
    }
}
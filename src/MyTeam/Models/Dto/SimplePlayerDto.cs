using System;
using MyTeam.Models.Enums;

namespace MyTeam.Models.Dto
{
    public class SimplePlayerDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string ImageSmall { get; set; }
        public PlayerStatus Status { get; set; }
      }
}
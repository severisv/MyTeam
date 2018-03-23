using System;
using MyTeam.Models.Enums;

namespace MyTeam.Models.Dto
{
    public class SimpleArticleDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Headline { get; set; }
        public DateTime Published { get; set; }
      }
}
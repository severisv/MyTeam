using System;
using System.Collections.Generic;

namespace MyTeam.Models.Domain
{
    public class Article : Entity
    {
        public string Headline { get; set; }
        public string Content { get; set; }
        public string ImageUrl { get; set; }
        public DateTime Published { get; set; }

        public virtual Guid AuthorId { get; set; }
        public virtual Member Author { get; set; }

        public Guid? GameId { get; set; }
        public virtual Game Game { get; set; }

    }
}
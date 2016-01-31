using System;

namespace MyTeam.Models.Domain
{
    public class Comment : Entity
    {
        public Guid ArticleId { get; set; }   
        public Guid AuthorId { get; set; }

        public DateTime Date { get; set; }
        public string Content { get; set; }

        public virtual Article Article { get; set; }
        public virtual ApplicationUser Author { get; set; }
    }
}
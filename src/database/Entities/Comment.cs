using System;

namespace MyTeam.Models.Domain
{
    public class Comment : Entity
    {
        public Guid ArticleId { get; set; }   
        public Guid? MemberId { get; set; }
        public DateTime Date { get; set; }
        public string AuthorUserName { get; set; }
        public string AuthorName { get; set; }
        public string AuthorFacebookId { get; set; }
        public string Content { get; set; }
        public virtual Article Article { get; set; }
        public virtual Member Member { get; set; }
    }
}
using System;

namespace MyTeam.ViewModels.News
{
    public class ArticleViewModel
    {
        public Guid Id { get; set; }
        public Guid? GameId { get; set; }
        public Guid? AuthorId { get; set; }
        public string Headline { get; set; }
        public string Content { get; set; }
        public string ImageUrl { get; set; }
        public DateTime Published { get; set; }
        public int CommentCount { get; set; }
        public MemberViewModel Author { get; set; }
    }
}
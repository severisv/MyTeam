using System;

namespace MyTeam.ViewModels.News
{
    public class CommentViewModel
    {
        public Guid ArticleId { get;  }
        public DateTime Date { get;  }
        public string Content { get;  }

        public CommentMemberViewModel Member { get; }

        public CommentViewModel(CommentMemberViewModel member, Guid articleId, DateTime date, string content)
        {
            ArticleId = articleId;
            Date = date;
            Content = content;
            Member = member;
        }
       

    }
}
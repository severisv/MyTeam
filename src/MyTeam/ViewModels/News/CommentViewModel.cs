using System;

namespace MyTeam.ViewModels.News
{
    public class CommentViewModel
    {
        public Guid ArticleId { get;  }
        public Guid AuthorId { get;  }

        public DateTime Date { get;  }
        public string Content { get;  }

        public CommentViewModel(Guid articleId, Guid authorId, DateTime date, string content)
        {
            ArticleId = articleId;
            AuthorId = authorId;
            Date = date;
            Content = content;
        }
       

    }


}
using System;
using MyTeam.Settings;

namespace MyTeam.ViewModels.News
{
    public class CommentViewModel
    {
        public Guid ArticleId { get;  }
        public DateTime Date { get;  }
        public string Content { get;  }

        public string AuthorName { get; }
        public string AuthorUserName { get; }
        public string AuthorFacebookId { get; }
        public CommentMemberViewModel Member { get; }


     

        public CommentViewModel(CommentMemberViewModel member, Guid articleId, DateTime date, string content, string name, string facebookId, string userName)
        {
            ArticleId = articleId;
            Date = date;
            Content = content;
            Member = member;
            AuthorFacebookId = facebookId;
            AuthorName = name;
            AuthorUserName = userName;
        }
       

    }
}
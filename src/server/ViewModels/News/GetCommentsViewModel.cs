using System;
using System.Collections.Generic;

namespace MyTeam.ViewModels.News
{
    public class GetCommentsViewModel
    {
       public Guid ArticleId { get; set; }
        public string FacebookId { get; set; }
        public IEnumerable<CommentViewModel> Comments { get; set; }

        public GetCommentsViewModel(IEnumerable<CommentViewModel> comments, Guid articleId, string facebookId)
        {
            Comments = comments;
            ArticleId = articleId;
            FacebookId = facebookId;
        }
    }

}
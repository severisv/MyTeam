using System;

namespace MyTeam.ViewModels.News
{
    public class CommentMemberViewModel
    {
        public string FullName { get;  }
        public string ImageSmall { get;  }
        public Guid? Id { get; }
        public string FacebookId { get; set; }

        public CommentMemberViewModel(string fullName, string imageSmall, Guid? memberId, string facebookId)
        {
            FullName = fullName;
            ImageSmall = imageSmall;
            Id = memberId;
            FacebookId = facebookId;
        }

    }
}
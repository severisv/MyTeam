using System;

namespace MyTeam.ViewModels.News
{
    public class CommentMemberViewModel
    {
        public string FullName { get;  }
        public string ImageSmall { get;  }
        public Guid? Id { get; }

        public CommentMemberViewModel(string fullName, string imageSmall, Guid? memberId)
        {
            FullName = fullName;
            ImageSmall = imageSmall;
            Id = memberId;
        }

    }
}
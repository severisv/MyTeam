namespace MyTeam.ViewModels.News
{
    public class CommentMemberViewModel
    {
        public string FullName { get;  }
        public string ImageSmall { get;  }

        public CommentMemberViewModel(string fullName, string imageSmall)
        {
            FullName = fullName;
            ImageSmall = imageSmall;
        }

    }
}
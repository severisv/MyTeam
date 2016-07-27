using System;

namespace MyTeam.ViewModels.News
{
    public class CommentMemberViewModel
    {
        public string FullName { get; set; }
        public string ImageSmall { get; set; }
        public Guid? Id { get; set; }
        public string FacebookId { get; set; }

        public bool Exists => Id != null;

        public CommentMemberViewModel(Models.Domain.Member member)
        {
            if (member != null)
            {
                FullName = member.Name;
                ImageSmall = member.Image;
                Id = member.Id;
                FacebookId = member.FacebookId;
            }
        
        }

        public CommentMemberViewModel()
        {
            
        }

    }
}
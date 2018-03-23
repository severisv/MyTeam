using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MyTeam;

namespace MyTeam.ViewModels.News
{
    public class PostCommentViewModel
    {
        [RequiredNO]
        public Guid ArticleId { get; set; }

        public bool IsFacebookUser { get; set; }
        public string FacebookId { get; set; }

        [Display(Name = "Navn")]
        public string Name { get; set; }

        [RequiredNO]
        [Display(Name = "Innhold")]
        [StringLength(1000, MinimumLength = 3, ErrorMessage = "Kommentaren m� v�re minst 4 tegn")]
        public string Content { get; set; }

    }
}
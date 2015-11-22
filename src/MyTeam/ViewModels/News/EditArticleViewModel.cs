using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using MyTeam.Models.Domain;
using MyTeam.Resources;

namespace MyTeam.ViewModels.Table
{
    public class EditArticleViewModel 
    {

        public Guid ArticleId { get; set; }
        public string Headline { get; set; }
        public string Content { get; set; }
        public string ImageUrl { get; set; }
        public DateTime Published { get; set; }

        public virtual Guid AuthorId { get; set; }
        public virtual Member Author { get; set; }

        public Guid? GameId { get; set; }
        public virtual Game Game { get; set; }
    }
}

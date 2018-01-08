using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyTeam.Models.Domain
{
    public class Article : Entity
    {

        [Required]
        public Guid ClubId { get; set; }
        public Guid? GameId { get; set; }
        public Guid? AuthorId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Headline { get; set; }
        [Required]
        public string Content { get; set; }
        public string ImageUrl { get; set; }
        [Required]
        public DateTime Published { get; set; }

        public virtual Member Author { get; set; }
        public virtual Game Game { get; set; }
        public virtual Club Club { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }

    }
}
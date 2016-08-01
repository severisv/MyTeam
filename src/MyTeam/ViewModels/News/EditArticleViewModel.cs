using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MyTeam.Filters;
using MyTeam.Models.Domain;
using MyTeam.Validation.Attributes;
using MyTeam.ViewModels.Game;

namespace MyTeam.ViewModels.News
{
    public class EditArticleViewModel : IValidatableObject
    {

        public Guid ArticleId { get; set; }
        [RequiredNO]
        public string Headline { get; set; }
        public string Name { get; set; }
        [RequiredNO]
        public string Content { get; set; }
        public string ImageUrl { get; set; }
        public Guid? GameId { get; set; }
        public bool IsNew => ArticleId == Guid.Empty;

        public bool IsMatchReport { get; set; }

        public DateTime? Published { get; set; }

        public IEnumerable<SimpleGame> Games { get; set; }

        public EditArticleViewModel()
        {
        }

        public EditArticleViewModel(ArticleViewModel article, IEnumerable<SimpleGame> games)
        {
            ArticleId = article.Id;
            Name = article.Name;
            Headline = article.Headline;
            Content = article.Content;
            ImageUrl = article.ImageUrl;
            GameId = article.GameId;
            IsMatchReport = GameId != null;
            Published = article.Published;
            Games = games;
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var result = new List<ValidationResult>();
            if (IsMatchReport && GameId == null)
            {
                result.Add(new ValidationResult("Kamprapporter må knyttes til en kamp", new[] { nameof(GameId) }));
            }
            return result;
        }
    }
}

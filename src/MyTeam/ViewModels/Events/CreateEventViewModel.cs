using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNet.Mvc.Rendering;
using MyTeam.Models.Enums;
using MyTeam.Resources;

namespace MyTeam.ViewModels.Events
{
    public class CreateEventViewModel : IValidatableObject
    {
        [Required]
        public EventType? Type { get; set; }
        
        [Required]
        [Display(Name = Res.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }

        [Required]
        [Display(Name = Res.Time)]
        [DisplayFormat(DataFormatString = @"{0:hh\:mm}", ApplyFormatInEditMode = true)]
        public TimeSpan Time { get; set; }
        [Required]
        [Display(Name = Res.Location)]
        public string Location { get; set; }

        [Display(Name = Res.Description)]
        public string Description { get; set; }

        public bool Weekly { get; set; }
        public DateTime? ToDate { get; set; }

        [Display(Name = Res.Mandatory)]
        public bool Mandatory { get; set; }

        public IEnumerable<EventType> EventTypes => Enum.GetValues(typeof (EventType)).Cast<EventType>().Where(e => e != EventType.Alle);



        public CreateEventViewModel()
        {
            Time = new TimeSpan(19,30,0);
            Date = DateTime.Now.Date.AddDays(4);
            Mandatory = true;
        }


        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var result = new List<ValidationResult>();

            if (Type == EventType.Trening)
            {
                if (Mandatory == null)
                    result.Add(new ValidationResult(Res.FieldRequired, new[] {nameof(Mandatory)}));
            }

            return result;
        }
    }
}
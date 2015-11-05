using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNet.Mvc.Rendering;
using MyTeam.Models.Domain;
using MyTeam.Models.Enums;
using MyTeam.Resources;
using MyTeam.Settings;

namespace MyTeam.ViewModels.Events
{
    public class CreateEventViewModel : IValidatableObject
    {

        [Required]
        public EventType Type { get; set; }
        
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
        
        public DateTime DateTime => Date + Time;
        
        public string Location { get; set; }

        [Display(Name = Res.Description)]
        public string Description { get; set; }

        [Display(Name = Res.Headline)]
        public string Headline { get; set; }

        [Display(Name = Res.Opponent)]
        public string Opponent { get; set; }

        [Display(Name = Res.Recurring)]
        public bool Recurring { get; set; }
        [Display(Name = Res.ToDate)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? ToDate { get; set; }

        [Display(Name = Res.Mandatory)]
        public bool Mandatory { get; set; }


        public bool IsEditMode => EventId.HasValue;
        
        public bool HasOccured => IsEditMode && DateTime < DateTime.Now;

        public IEnumerable<EventType> EventTypes => Enum.GetValues(typeof (EventType)).Cast<EventType>().Where(e => e != EventType.Alle);
        public Guid? EventId { get; set; }


        public CreateEventViewModel()
        {
            Time = new TimeSpan(19,30,0);
            Date = DateTime.Now.Date.AddDays(4);
            Mandatory = true;
        }

        public CreateEventViewModel(Event ev)
        {
            var game = ev as Game;
            var opponent = game?.Opponent;

            var training = ev as Training;
            var voluntary = training?.Voluntary;

            Type = ev.Type;
            Date = ev.DateTime.Date;
            Time = ev.DateTime.TimeOfDay;
            Description = ev.Description;
            Headline = ev.Headline;
            Opponent = opponent;
            Location = ev.Location;
            EventId = ev.Id;
            Mandatory = !voluntary ?? false;
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var result = new List<ValidationResult>();

            if (Type == EventType.Trening)
            {
                if (Recurring)
                {
                    if (ToDate == null)
                    {
                        result.Add(new ValidationResult(Res.FieldRequired, new[] { nameof(ToDate) }));
                    }
                    else if (ToDate > DateTime.Now.AddMonths(Config.AllowedMonthsAheadInTimeForTrainingCreation))
                    {
                        ToDate = DateTime.Now.Date.AddMonths(Config.AllowedMonthsAheadInTimeForTrainingCreation);
                        result.Add(new ValidationResult(Res.TrainingCreationTooFarAheadInTime, new[] { nameof(ToDate) }));
                    }

                }
                    
            }

            return result;
        }

        public List<Event> CreateEvents()
        {
            var result = new List<Event>
            {
                CreateEvent(Date)
            };
            
            if (Recurring && ToDate.HasValue)
            {
                for (var date = Date.AddDays(7); date < ToDate.Value.Date.AddDays(1); date = date.AddDays(7))
                {
                    result.Add(CreateEvent(date));
                }
            }

            return result;

        }

        private Event CreateEvent(DateTime date)
        {
            if (Type == EventType.Kamp)
            {
                return new Game
                {
                    Location = Location,
                    Type = Type,
                    DateTime = date + Time,
                    Description = Description,
                    Headline = Headline,
                    Opponent = Opponent,
                };
            }
            else if (Type == EventType.Trening)
            {
                return new Training
                {
                    Location = Location,
                    Type = Type,
                    DateTime = date + Time,
                    Description = Description,
                    Voluntary = !Mandatory,
                    Headline = Headline,
                };
            }

            return new Event
            {
                Location = Location,
                Type = Type,
                DateTime = date + Time,
                Description = Description,
                Headline = Headline,
            };
        }

        public void UpdateEvent(Event ev)
        {
            ev.Location = Location;
            ev.DateTime = Date + Time;
            ev.Description = Description;
            ev.Headline = Headline;

            var training = ev as Training;
            if(training != null) training.Voluntary = !Mandatory;


            var game = ev as Game;
            if(game != null) game.Opponent = Opponent;
        }
    }
}
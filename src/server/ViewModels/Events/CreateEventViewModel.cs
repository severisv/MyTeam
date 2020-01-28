using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using MyTeam.Models.Domain;
using MyTeam.Models.Enums;
using MyTeam.Resources;
using MyTeam.Settings;
using MyTeam;

namespace MyTeam.ViewModels.Events
{
    public class CreateEventViewModel : IValidatableObject
    {

        [RequiredNO]
        public EventType Type { get; set; }

        [RequiredNO]
        [Display(Name = Res.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public string Date { get; set; }

        [RequiredNO]
        [Display(Name = Res.Time)]
        [DisplayFormat(DataFormatString = @"{0:hh\:mm}", ApplyFormatInEditMode = true)]
        public string Time { get; set; }
        [Display(Name = Res.Time)]

        public DateTime DateTime => (Date.AsDate() ?? DateTime.MinValue) + (Time.AsTime() ?? TimeSpan.Zero);
        [Display(Name = Res.Location)]
        [RequiredNO]
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
        public string ToDate { get; set; }

        [Display(Name = Res.Mandatory)]
        public bool Mandatory { get; set; }

        [Display(Name = Res.Team)]
        public IList<Guid> TeamIds { get; set; }

        [Display(Name = Res.Team)]
        public IList<TeamViewModel> Teams { get; set; }


        [Display(Name = Res.Type)]
        public GameType? GameType { get; set; }

        public GameType[] GameTypes => Enum.GetValues(typeof(GameType)).Cast<GameType>().ToArray();

        public bool IsEditMode => EventId.HasValue;

        public bool HasOccured => IsEditMode && DateTime < DateTime.Now;

        public IEnumerable<EventType> EventTypes => Enum.GetValues(typeof(EventType)).Cast<EventType>().Where(e => e != EventType.Alle);
        public Guid? EventId { get; set; }

        [Display(Name = Res.HomeGround)]
        public bool IsHomeTeam { get; set; }


        public CreateEventViewModel()
        {
            Time = new TimeSpan(19, 30, 0).ToString();
            Date = DateTime.Now.Date.AddDays(4).ToNoFull();
            Mandatory = true;
            IsHomeTeam = true;
            TeamIds = new List<Guid>();
        }

        public CreateEventViewModel(EventViewModel ev)
        {
            var opponent = ev.Opponent;
            var voluntary = ev.Voluntary;

            Type = ev.Type;
            Date = ev.DateTime.Date.ToNoFull();
            Time = ev.DateTime.TimeOfDay.ToNo();
            Description = ev.Description;
            Headline = ev.Headline;
            Opponent = opponent;
            Location = ev.Location;
            EventId = ev.Id;
            Mandatory = !voluntary;
            TeamIds = ev.TeamIds.ToList();
            IsHomeTeam = ev.IsHomeTeam;
            GameType = ev.GameType;
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var result = new List<ValidationResult>();

            if (Date.AsDate() == null)
            {
                result.Add(new ValidationResult("Dato må være på formatet dd.mm.åååå", new[] { nameof(Date) }));
            }
            if (!string.IsNullOrEmpty(ToDate) && ToDate.AsDate() == null)
            {
                result.Add(new ValidationResult("Til-dato må være på formatet dd.mm.åååå", new[] { nameof(ToDate) }));
            }


            if (Type == EventType.Trening)
            {
                if (Recurring)
                {
                    if (ToDate == null)
                    {
                        result.Add(new ValidationResult(Res.FieldRequired, new[] { nameof(ToDate) }));
                    }
                    else if (ToDate.AsDate() > DateTime.Now.AddMonths(Config.AllowedMonthsAheadInTimeForTrainingCreation))
                    {
                        ToDate = DateTime.Now.Date.AddMonths(Config.AllowedMonthsAheadInTimeForTrainingCreation).ToNoFull();
                        result.Add(new ValidationResult(Res.TrainingCreationTooFarAheadInTime, new[] { nameof(ToDate) }));
                    }
                }
            }
            else if (Type == EventType.Kamp)
            {
                if (string.IsNullOrWhiteSpace(Opponent))
                {
                    result.Add(new ValidationResult(Res.FieldRequired, new[] { nameof(Opponent) }));
                }
            }
            else if (Type == EventType.Diverse)
            {
                if (string.IsNullOrWhiteSpace(Headline))
                {
                    result.Add(new ValidationResult(Res.FieldRequired, new[] { nameof(Headline) }));
                }
            }

            if (!TeamIds.Any(t => t != Guid.Empty))
            {
                result.Add(new ValidationResult(Res.FieldRequired, new[] { nameof(TeamIds) }));
            }

            if (Time.AsTime() == null)
            {
                result.Add(new ValidationResult("Skriv inn et gyldig tidspunkt på formatet tt:mm", new[] { nameof(Time) }));
            }
            return result;
        }

        public List<Event> CreateEvents()
        {
            var result = new List<Event>
            {
                CreateEvent(Date)
            };

            if (Recurring && ToDate.AsDate().HasValue)
            {
                for (var date = Date.AsDate().Value.AddDays(7); date < ToDate.AsDate().Value.Date.AddDays(1); date = date.AddDays(7))
                {
                    result.Add(CreateEvent(date.ToNoFull()));
                }
            }

            return result;

        }

        private Event CreateEvent(string dateTime)
        {
            var date = (DateTime)dateTime.AsDate();
            var eventId = EventId ?? Guid.NewGuid();
            var eventTeams = new List<EventTeam>();
            foreach (var id in TeamIds)
            {
                eventTeams.Add(new EventTeam
                {
                    Id = Guid.NewGuid(),
                    TeamId = id,
                    EventId = eventId
                });
            }

            var ev = Type == EventType.Kamp ? new Models.Domain.Event()
            {
                TeamId = TeamIds.Single()
            } : new Event();

            ev.Id = eventId;
            ev.Location = Location;
            ev.Type = Type.ToInt();
            ev.DateTime = date + Time.AsTime().Value;
            ev.Description = Description;
            ev.Headline = Headline;
            ev.Opponent = Opponent;
            ev.EventTeams = eventTeams;
            ev.Voluntary = !Mandatory;
            ev.GameTypeValue = GameType;
            ev.IsHomeTeam = IsHomeTeam;


            return ev;
        }

    }
}
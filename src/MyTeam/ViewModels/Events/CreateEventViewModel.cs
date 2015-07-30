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
    public class CreateEventViewModel
    {
        [Required]
        public EventType? Type { get; set; }
        
        [Required]
        //[DisplayName(Res.Date)]
        public DateTime Date { get; set; }

        [Required]
        //[DisplayName(Res.Time)]
        public TimeSpan Time { get; set; }
        [Required]
        //[DisplayName(Res.Location)]
        public string Location { get; set; }

        public string Description { get; set; }

        public bool Weekly { get; set; }
        public DateTime? ToDate { get; set; }

        public IEnumerable<EventType> EventTypes => Enum.GetValues(typeof (EventType)).Cast<EventType>().Where(e => e != EventType.Alle);



        public CreateEventViewModel()
        {
            Time = new TimeSpan(0,19,30);
            Date = DateTime.Now.Date.AddDays(4);
        }

    }
}
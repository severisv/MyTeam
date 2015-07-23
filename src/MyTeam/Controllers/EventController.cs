using System;
using System.Collections;
using System.Linq;
using Microsoft.AspNet.Mvc;
using MyTeam.Models.Domain;
using MyTeam.Models.Enums;
using MyTeam.Resources;
using MyTeam.Services.Domain;
using MyTeam.Services.Repositories;
using MyTeam.ViewModels.Player;
using MyTeam.ViewModels.Training;


namespace MyTeam.Controllers
{
    public class EventController : Controller
    {
        [FromServices]
        public IEventService EventService { get; set; }


        public IActionResult Index(EventType type = EventType.Alle)
        {
            var events = EventService.GetUpcoming(type);

            var model = new UpcomingEventsViewModel(events, type);

            return View(model);
        }
    

       

    
    }
}

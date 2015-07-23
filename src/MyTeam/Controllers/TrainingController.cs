using System;
using System.Collections;
using System.Linq;
using Microsoft.AspNet.Mvc;
using MyTeam.Models.Domain;
using MyTeam.Resources;
using MyTeam.Services.Domain;
using MyTeam.Services.Repositories;
using MyTeam.ViewModels.Player;
using MyTeam.ViewModels.Training;


namespace MyTeam.Controllers
{
    public class TrainingController : Controller
    {
        [FromServices]
        public IEventService<Training> TrainingService { get; set; }


        public IActionResult Index(PlayerStatus type = PlayerStatus.Aktiv)
        {
            var trainings = TrainingService.GetUpcoming();

            var model = new ShowTrainingsViewModel(trainings);

            return View(model);
        }
    

       

    
    }
}

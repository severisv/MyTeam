using System;
using Microsoft.AspNet.Mvc;
using MyTeam.Models.Domain;
using MyTeam.Models.Enums;
using MyTeam.Services.Domain;
using MyTeam.ViewModels.Events;


namespace MyTeam.Controllers
{
   

    public class BaseController : Controller
    {
        public Player ActivePlayer { get; set; }
    }
}

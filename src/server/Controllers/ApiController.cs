using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using MyTeam.Filters;
using MyTeam.Models;
using MyTeam.Models.Enums;
using MyTeam.Models.Structs;
using MyTeam.Services.Application;
using MyTeam.Services.Domain;


namespace MyTeam.Controllers
{
    [RequireMember]
    public class ApiController : BaseController
    {

        private readonly ApplicationDbContext _dbContext;
        private readonly IEventService _eventService;
        private readonly IPlayerService _playerService;
        private readonly ICacheHelper _cacheHelper;

        public ApiController(ApplicationDbContext dbContext, IEventService eventService, IPlayerService playerService, ICacheHelper cacheHelper)
        {
            _playerService = playerService;
            _eventService = eventService;
            _dbContext = dbContext;
            _cacheHelper = cacheHelper;
        }


        [RequireMember(Roles.Coach, Roles.Admin)]
        public IActionResult Now()
        {
            return Json(new
            {
                DateTime.Now,
                Date = DateTime.Now.ToShortDateString(),
                Time = DateTime.Now.ToShortTimeString()
            });
        }


        [RequireMember(Roles.Coach, Roles.Admin)]
        public IActionResult TestException()
        {
            throw new Exception("Boom");
        }


    }
}

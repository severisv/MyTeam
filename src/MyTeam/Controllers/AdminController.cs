﻿using System.Linq;
using Microsoft.AspNet.Mvc;
using Microsoft.Framework.Configuration;
using MyTeam.Models.Structs;
using MyTeam.Resources;
using MyTeam.Services.Domain;
using MyTeam.ViewModels.Admin;

namespace MyTeam.Controllers
{
    public class AdminController : BaseController
    {
        [FromServices]
        public IConfiguration Configuration { get; set; }
        [FromServices]
        public IPlayerService PlayerService { get; set; }



        public IActionResult Index()
        {

            var facebookAppId = Configuration["Authentication:Facebook:AppId"];

            var model = new AdminViewModel(facebookAppId);

            return View(model);
        }

        public IActionResult AddPlayers()
        {

            var facebookAppId = Configuration["Authentication:Facebook:AppId"];

            var model = new AdminViewModel(facebookAppId);

            return View(model);
        }

        [HttpPost]
        public JsonResult AddPlayer(AddPlayerViewModel model)
        {
            if (ModelState.IsValid)
            {
                var response = PlayerService.Add(Club.ClubId, model.FacebookId, model.FirstName, model.LastName, model.EmailAddress, model.ImageSmall, model.ImageMedium, model.ImageLarge);
                return new JsonResult(response);
           }

            var validationMessage = string.Join(" ,", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            return new JsonResult(JsonResponse.ValidationFailed(validationMessage));
        }

        public JsonResult GetFacebookIds()
        {
            var ids = PlayerService.GetFacebookIds();
            return new JsonResult(new { data = ids});
        }

    }
}  
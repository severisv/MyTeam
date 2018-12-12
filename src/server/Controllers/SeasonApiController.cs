using System;
using Microsoft.AspNetCore.Mvc;
using MyTeam.Services.Domain;

namespace MyTeam.Controllers
{
    public class SeasonApiController : Controller
    {
        private readonly IFixtureService _fixtureService;

        public SeasonApiController(IFixtureService fixtureService)
        {
            _fixtureService = fixtureService;
        }


        public IActionResult Refresh()
        {
            _fixtureService.RefreshFixtures();
            return Ok("200 OK");
        }


    }
}
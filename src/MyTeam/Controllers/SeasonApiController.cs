using System;
using Microsoft.AspNetCore.Mvc;
using MyTeam.Services.Domain;

namespace MyTeam.Controllers
{
    public class SeasonApiController : Controller
    {
        private readonly ITableService _tableService;
        private readonly IFixtureService _fixtureService;

        public SeasonApiController(ITableService tableService, IFixtureService fixtureService)
        {
            _tableService = tableService;
            _fixtureService = fixtureService;
        }


        public void Refresh(Guid seasonId)
        {

            _tableService.RefreshTables();
            _fixtureService.RefreshFixtures();
        }


    }
}
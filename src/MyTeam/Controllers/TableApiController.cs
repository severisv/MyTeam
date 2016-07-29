using System;
using Microsoft.AspNetCore.Mvc;
using MyTeam.Services.Domain;

namespace MyTeam.Controllers
{
    public class TableApiController : Controller
    {
        private readonly ITableService _tableService;
        private readonly IFixtureService _fixtureService;

        public TableApiController(ITableService tableService, IFixtureService fixtureService)
        {
            _tableService = tableService;
            _fixtureService = fixtureService;
        }


        public void RefreshTables(Guid seasonId)
        {

            _tableService.RefreshTables();
            _fixtureService.RefreshFixtures();
        }


    }
}
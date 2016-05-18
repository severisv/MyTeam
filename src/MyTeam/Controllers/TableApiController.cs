using System;
using Microsoft.AspNetCore.Mvc;
using MyTeam.Services.Domain;

namespace MyTeam.Controllers
{
    public class TableApiController : Controller
    {
        private readonly ITableService _tableService;

        public TableApiController(ITableService tableService)
        {
            _tableService = tableService;
        }


        public void RefreshTables(Guid seasonId)
        {

            _tableService.RefreshTables();
        }


    }
}
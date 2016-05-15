using System;
using System.Linq;
using Microsoft.AspNet.Mvc;
using MyTeam.Filters;
using MyTeam.Models;
using MyTeam.Models.Enums;
using MyTeam.Services.Domain;
using MyTeam.ViewModels.Table;

namespace MyTeam.Controllers
{
    public class TableApiController : Controller
    {
        [FromServices]
        public ISeasonService SeasonService { get; set; }
        [FromServices]
        public ITableService TableService { get; set; }
        [FromServices]
        public ApplicationDbContext DbContext { get; set; }

        public void RefreshTables(Guid seasonId)
        {

            TableService.RefreshTables();
        }


    }
}
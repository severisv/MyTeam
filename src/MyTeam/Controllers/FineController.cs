using System;
using Microsoft.AspNetCore.Mvc;
using MyTeam.Filters;
using MyTeam.Models.Structs;
using MyTeam.Services.Domain;


namespace MyTeam.Controllers
{
    [RequireMember]
    [Route("intern/bot")]
    public class FineController : BaseController
    {

        private readonly IFineService _fineService;

        public FineController(IFineService fineService)
        {
            _fineService = fineService;
        }

        [Route("satser")]
        public IActionResult Rates()
        {
            var remedyRates = _fineService.GetRates(Club.Id);

            return View(remedyRates);

        }


        [Route("satser/slett/{id}")]
        public JsonResult Delete(Guid id)
        {
            _fineService.DeleteRate(id);

            return new JsonResult(JsonResponse.Success());

        }
    }
}

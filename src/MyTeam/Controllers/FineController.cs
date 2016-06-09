using System;
using Microsoft.AspNetCore.Mvc;
using MyTeam.Filters;
using MyTeam.Services.Domain;
using MyTeam.Models.Domain;

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


        [Route("satser/slett/{rateId}")]
        public IActionResult Delete(Guid rateId)
        {
            _fineService.DeleteRate(rateId);

           return RedirectToAction("Index");
        }

        [Route("satser/leggtil")]
        public IActionResult Add(RemedyRate rate)
        {
            _fineService.AddRate(rate);

            return View("_ShowRate", rate);

        }
    }
}

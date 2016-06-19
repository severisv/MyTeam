using System;
using System.Threading;
using Microsoft.AspNetCore.Mvc;
using MyTeam.Filters;
using MyTeam.Services.Domain;
using MyTeam.Models.Domain;
using MyTeam.ViewModels.Fine;

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

           return RedirectToAction("Rates");
        }

        [Route("satser/leggtil")]
        [HttpPost]
        public IActionResult Add(RemedyRateViewModel model)
        {
            if (Request.IsAjaxRequest())
            {
                if (ModelState.IsValid)
                {
                    model.Id = Guid.NewGuid();
                    _fineService.AddRate(Club.Id, model);
                    return PartialView("_ShowRate", model);
                }

                return PartialView("_ShowRate", null);
            }
           
            if (ModelState.IsValid)
            {
                _fineService.UpdateRate(model);
                return RedirectToAction("Rates");
            }
            return PartialView("Edit", model);

        }

        [Route("satser/endre")]
        public IActionResult Edit(Guid rateId)
        {
            var model = _fineService.GetRate(rateId);
            return View(model);

        }
       
    }
}

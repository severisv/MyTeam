using System;
using Microsoft.AspNetCore.Mvc;
using MyTeam.Filters;
using MyTeam.Services.Domain;
using MyTeam.ViewModels.RemedyRate;

namespace MyTeam.Controllers
{
    [RequireMember]
    [Route("intern/boter/satser")]
    public class RemedyRateController : BaseController
    {

        private readonly IRemedyRateService _remedyRateService;

        public RemedyRateController(IRemedyRateService remedyRateService)
        {
            _remedyRateService = remedyRateService;
        }

        [Route("")]
        public IActionResult Index()
        {
            var remedyRates = _remedyRateService.GetRates(Club.Id);

            return View(remedyRates);

        }


        [Route("slett/{rateId}")]
        public IActionResult Delete(Guid rateId)
        {
            _remedyRateService.Delete(rateId);

           return RedirectToAction("Index");
        }

        [Route("endre")]
        [HttpPost]
        public IActionResult Add(RemedyRateViewModel model)
        {
            if (Request.IsAjaxRequest())
            {
                if (ModelState.IsValid)
                {
                    model.Id = Guid.NewGuid();
                    _remedyRateService.Add(Club.Id, model);
                    return PartialView("_ShowRate", model);
                }

                return PartialView("_ShowRate", null);
            }
           
            if (ModelState.IsValid)
            {
                _remedyRateService.Update(model);
                return RedirectToAction("Index");
            }
            return View("Edit", model);

        }

        [Route("endre")]
        public IActionResult Edit(Guid rateId)
        {
            var model = _remedyRateService.Get(rateId);
            return View(model);

        }
       
    }
}

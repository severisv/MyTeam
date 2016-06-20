using System;
using Microsoft.AspNetCore.Mvc;
using MyTeam.Filters;
using MyTeam.Services.Domain;
using MyTeam.ViewModels.Fine;
using MyTeam.Models.Enums;

namespace MyTeam.Controllers
{
    [RequireMember]
    [Route("intern/boter")]
    public class FineController : BaseController
    {

        private readonly IFineService _fineService;
        private readonly IPlayerService _playerService;
        private readonly IRemedyRateService _rateService;


        public FineController(IFineService fineService, IRemedyRateService rateService, IPlayerService playerService)
        {
            _fineService = fineService;
            _playerService = playerService;
            _rateService = rateService;
        }

        [Route("vis/{aar:int?}/{spillerId?}")]
        public IActionResult List(int? aar = null, Guid? memberId = null)
        {
            var fines = _fineService.Get(Club.Id, aar ?? DateTime.Now.Year, memberId);
            var rates = _rateService.GetRates(Club.Id);
            var players = _playerService.GetDto(Club.Id, PlayerStatus.Aktiv);

            var model = new ListFineViewModel(fines, rates, players);
            return View(model);

        }


        [Route("slett/{rateId}")]
        public IActionResult Delete(Guid rateId)
        {
            _fineService.Delete(rateId);

            return RedirectToAction("List");
        }

        [Route("leggtil")]
        [HttpPost]
        public IActionResult Add(AddFineViewModel model)
        {
            if (ModelState.IsValid)
            {
                var fineId = _fineService.Add(model);
                var fine = _fineService.Get(fineId);
                return PartialView("_Show", fine);
            }

            return PartialView("_Show", null);
        }

        [Route("betalt")]
        [HttpPost]
        public void SetPaid(Guid fineId, bool value)
        {
            _fineService.SetPaid(fineId, value);
        }

    }
}

using System;
using System.Linq;
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
        private readonly IPaymentService _paymentService;


        public FineController(IFineService fineService, IRemedyRateService rateService, IPlayerService playerService, IPaymentService paymentService)
        {
            _fineService = fineService;
            _playerService = playerService;
            _rateService = rateService;
            _paymentService = paymentService;
        }

        [Route("{aar:int?}")]
        public IActionResult Index(int? aar = null)
        {
            var year = aar ?? DateTime.Now.Year;
            var years = _fineService.GetYears(Club.Id);
            var fines = _fineService.Get(Club.Id, year);
            var payments = _paymentService.Get(Club.Id, year);
            var currentUserDue = _fineService.GetDueAmount(CurrentMember.Id);
            var paymentInfo = _fineService.GetPaymentInformation(Club.Id);
            var paymentInfoModel = new PaymentInfoViewModel(CurrentMember.Image, CurrentMember.FacebookId, paymentInfo, currentUserDue);
            var model = new IndexViewModel(years, year, fines, payments, paymentInfoModel);
            return View(model);

        }


        [Route("vis/{aar:int?}/{memberId?}")]
        public IActionResult List(int? aar = null, Guid? memberId = null)
        {
            var year = aar ?? DateTime.Now.Year;
            var fines = _fineService.Get(Club.Id, year, memberId);
            var rates = _rateService.GetRates(Club.Id);
            var players = _playerService.GetDto(Club.Id, PlayerStatus.Aktiv).ToList();
            var years = _fineService.GetYears(Club.Id);
            var selectedPlayer = players.FirstOrDefault(p => p.Id == memberId);

            var model = new ListFineViewModel(fines, rates, players, years, year, selectedPlayer);
            return View(model);

        }


        [Route("slett/{rateId}")]
        [RequireMember(Roles.Finemaster)]
        public IActionResult Delete(Guid rateId)
        {
            _fineService.Delete(rateId);

            return RedirectToAction("List");
        }

        [Route("leggtil")]
        [HttpPost]
        [RequireMember(Roles.Finemaster)]
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

      
        [Route("betalingsinformasjon")]
        [HttpPost]
        [RequireMember(Roles.Finemaster)]
        public void SetPaymentInfo(string value) => _fineService.UpdatePaymentInformation(Club.Id, value);        

    }
}

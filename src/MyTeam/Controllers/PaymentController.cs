using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using MyTeam.Filters;
using MyTeam.Services.Domain;
using MyTeam.ViewModels.Fine;
using MyTeam.Models.Enums;
using MyTeam.ViewModels.Payment;

namespace MyTeam.Controllers
{
    [RequireMember]
    [Route("intern/boter")]
    public class PaymentController : BaseController
    {

        private readonly IPaymentService _paymentService;
        private readonly IPlayerService _playerService;


        public PaymentController(IPaymentService paymentService, IPlayerService playerService)
        {
            _paymentService = paymentService;
            _playerService = playerService;
        }
        
        [Route("{aar:int?}/{memberId?}")]
        public IActionResult Index(int? aar = null, Guid? memberId = null)
        {
            var year = aar ?? DateTime.Now.Year;
            var payments = _paymentService.Get(Club.Id, year, memberId);
            var players = _playerService.GetDto(Club.Id, PlayerStatus.Aktiv).ToList();
            var years = _paymentService.GetYears(Club.Id);
            var selectedPlayer = players.FirstOrDefault(p => p.Id == memberId);

            var model = new ListPaymentViewModel(payments, players, years, year, selectedPlayer);
            return View(model);

        }
        
        [Route("slett/{paymentId}")]
        [RequireMember(Roles.Finemaster)]
        public IActionResult Delete(Guid rateId)
        {
            _paymentService.Delete(rateId);

            return RedirectToAction("Index");
        }

        [Route("leggtil")]
        [HttpPost]
        [RequireMember(Roles.Finemaster)]
        public IActionResult Add(AddPaymentViewModel model)
        {
            if (ModelState.IsValid)
            {
                var fineId = _paymentService.Add(model, Club.Id);
                var fine = _paymentService.Get(fineId);
                return PartialView("_Show", fine);
            }

            return PartialView("_Show", null);
        }
        
    }
}

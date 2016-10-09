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
    [RequireMember(Roles.Finemaster)]
    [Route("intern/innbetalinger")]
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
            var payments = _paymentService.GetPayments(Club.Id, year, memberId);
            var players = _playerService.GetDto(Club.Id, PlayerStatus.Aktiv).ToList();
            var years = _paymentService.GetYears(Club.Id);
            var selectedPlayer = players.FirstOrDefault(p => p.Id == memberId);

            var model = new ListPaymentViewModel(payments, players, years, year, selectedPlayer);
            return View(model);

        }
        
        [Route("slett/{paymentId}")]
        public IActionResult Delete(Guid paymentId, int? aar = null, Guid? memberId = null)
        {
            _paymentService.Delete(paymentId);

            return RedirectToAction("Index", new { aar, memberId });
        }

        [Route("leggtil")]
        [HttpPost]
        [RequireMember(Roles.Finemaster)]
        public IActionResult Add(AddPaymentViewModel model)
        {
            if (ModelState.IsValid)
            {
                var paymentId = _paymentService.Add(model, Club.Id);
                var payment = _paymentService.Get(paymentId);
                return PartialView("_Show", payment);
            }

            return PartialView("_Show", null);
        }
        
    }
}

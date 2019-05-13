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


        public FineController(IFineService fineService)
        {
            _fineService = fineService;
        }
     
        public class SetPaymentInfoModel
        {
            public string value;
        }

        [Route("betalingsinformasjon")]
        [HttpPost]
        [RequireMember(Roles.Finemaster)]
        public void SetPaymentInfo([FromBody] SetPaymentInfoModel value) => _fineService.UpdatePaymentInformation(Club.Id, value.value);

    }
}

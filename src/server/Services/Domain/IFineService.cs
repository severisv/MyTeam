using System;
using System.Collections.Generic;
using MyTeam.ViewModels.Fine;

namespace MyTeam.Services.Domain
{
    public interface IFineService
    {
        string GetPaymentInformation(Guid clubId);
        void UpdatePaymentInformation(Guid clubId, string paymentInformation);
    }

}
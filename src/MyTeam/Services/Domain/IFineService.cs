using System;
using System.Collections.Generic;
using MyTeam.ViewModels.Fine;

namespace MyTeam.Services.Domain
{
    public interface IFineService
    {
        IEnumerable<FineViewModel> Get(Guid clubId, int year, Guid? memberId = null);
        void Delete(Guid fineId);
        Guid Add(AddFineViewModel rate);
        FineViewModel Get(Guid rateId);
        IEnumerable<int> GetYears(Guid id);
        string GetPaymentInformation(Guid clubId);
        void UpdatePaymentInformation(Guid clubId, string paymentInformation);
        int GetDueAmount(Guid memberId);
    }

}
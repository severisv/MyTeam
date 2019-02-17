using System;
using System.Collections.Generic;
using MyTeam.ViewModels.Fine;

namespace MyTeam.Services.Domain
{
    public interface IFineService
    {
        IEnumerable<FineViewModel> GetFines(Guid clubId, int? year = null, Guid? memberId = null);
        void Delete(Guid fineId);
        Guid Add(AddFineViewModel rate);
        FineViewModel Get(Guid rateId);
        IEnumerable<int> GetYears(Guid id);
        string GetPaymentInformation(Guid clubId);
        void UpdatePaymentInformation(Guid clubId, string paymentInformation);
    }

}
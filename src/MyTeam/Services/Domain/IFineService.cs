using System;
using System.Collections.Generic;
using MyTeam.Models.Domain;
using MyTeam.ViewModels.Fine;

namespace MyTeam.Services.Domain
{
    public interface IFineService
    {
        IEnumerable<RemedyRateViewModel> GetRates(Guid clubId);
        void UpdateRate(RemedyRateViewModel rate);
        void DeleteRate(Guid rateId);
        void AddRate(Guid clubId, RemedyRateViewModel rate);
        RemedyRateViewModel GetRate(Guid rateId);
    }
}
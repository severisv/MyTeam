using System;
using System.Collections.Generic;
using MyTeam.ViewModels.RemedyRate;

namespace MyTeam.Services.Domain
{
    public interface IRemedyRateService
    {
        IEnumerable<RemedyRateViewModel> GetRates(Guid clubId);
        void Update(RemedyRateViewModel rate);
        void Delete(Guid rateId);
        void Add(Guid clubId, RemedyRateViewModel rate);
        RemedyRateViewModel Get(Guid rateId);
    }
}
using System;
using System.Collections.Generic;
using MyTeam.Models.Domain;

namespace MyTeam.Services.Domain
{
    public interface IFineService
    {
        IEnumerable<RemedyRate> GetRates(Guid id);
        void DeleteRate(Guid rateId);
        void AddRate(RemedyRate rate);
    }
}
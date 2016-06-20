using System;
using System.Collections.Generic;
using MyTeam.ViewModels.Fine;

namespace MyTeam.Services.Domain
{
    public interface IFineService
    {
        IEnumerable<FineViewModel> Get(Guid clubId, int year, Guid? memberId);
        void Delete(Guid rateId);
        Guid Add(AddFineViewModel rate);
        FineViewModel Get(Guid rateId);
        void SetPaid(Guid fineId, bool value);
    }
}
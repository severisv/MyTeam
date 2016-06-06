using System;
using System.Collections.Generic;
using MyTeam.Models.Domain;

namespace MyTeam.Services.Domain
{
    class FineService : IFineService
    {
        public void DeleteRate(Guid rateId)
        {

        }

        public IEnumerable<RemedyRate> GetRates(Guid id)
        {
            return new List<RemedyRate>
            {
                new RemedyRate
                {
                    Description = "Boten er hei",
                    Name="Forsentkomming",
                    Rate = 1337
                }
            };
        }
    }
}
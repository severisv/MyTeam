using System;
using System.Collections.Generic;
using System.Linq;
using MyTeam.Models.Domain;
using MyTeam.Models;
using MyTeam.Services.Application;
using MyTeam.ViewModels.Fine;

namespace MyTeam.Services.Domain
{
    class FineService : IFineService
    {

        private readonly ApplicationDbContext _dbContext;

        public FineService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
     

        public string GetPaymentInformation(Guid clubId)
        {
            return _dbContext.PaymentInformation.FirstOrDefault(s => s.ClubId == clubId)?.Info ?? String.Empty;
        }

        public void UpdatePaymentInformation(Guid clubId, string paymentInformation)
        {
            var paymentInfo = _dbContext.PaymentInformation.FirstOrDefault(s => s.ClubId == clubId);

            if (paymentInfo == null)
            {
                paymentInfo = new PaymentInformation
                {
                    Id = Guid.NewGuid(),
                    ClubId = clubId
                };
                _dbContext.PaymentInformation.Add(paymentInfo);
            }
            paymentInfo.Info = paymentInformation;
            _dbContext.SaveChanges();
        }
        
    }
}
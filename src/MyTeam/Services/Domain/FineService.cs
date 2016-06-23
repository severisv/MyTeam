using System;
using System.Collections.Generic;
using System.Linq;
using MyTeam.Models.Domain;
using MyTeam.Models;
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

        public void Delete(Guid fineId)
        {
            var fine = _dbContext.Fines.Single(r => r.Id == fineId);
            _dbContext.Fines.Remove(fine);
            _dbContext.SaveChanges();
        }

        public Guid Add(AddFineViewModel model)
        {
            var rate = _dbContext.RemedyRates.Single(r => r.Id == model.RateId);

            var fine = new Fine
            {
                Id = Guid.NewGuid(),
                Amount = (model.ExtraRate ?? 0) + rate.Rate,
                Comment = model.Comment,
                MemberId = model.MemberId.Value,
                RemedyRateId = model.RateId.Value,
                Issued = DateTime.Now,
                RateName = rate.Name,
            };
            _dbContext.Add(fine);
            _dbContext.SaveChanges();
            return fine.Id;
        }

        public FineViewModel Get(Guid id)
        {
            var query = _dbContext.Fines.Where(r => r.Id == id);

            return Select(query).Single();
        }

        public IEnumerable<FineViewModel> Get(Guid clubId, int year, Guid? memberId)
        {
            var query = _dbContext.Fines.Where(f => f.Rate.ClubId == clubId && f.Issued.Year == year);

            if (memberId != null) query = query.Where(f => f.MemberId == memberId);

            return Select(query).OrderByDescending(f => f.Issued);

        }

        private static List<FineViewModel> Select(IQueryable<Fine> query)
        {
            return query.Select(f => new FineViewModel
            {
                MemberId = f.MemberId,
                Id = f.Id,
                Description = f.RateName,
                Name = f.Member.Name,
                Issued = f.Issued,
                PaidDate = f.Paid,
                Rate = f.Amount,
                Comment = f.Comment,
                FirstName = f.Member.FirstName,
                LastName = f.Member.LastName,
                MemberImage = f.Member.Image,
                FacebookId = f.Member.FacebookId
            }).ToList();

        }

        public void SetPaid(Guid fineId, bool value)
        {
            var fine = _dbContext.Fines.Single(f => f.Id == fineId);
            fine.Paid = value ? (DateTime?) DateTime.Now : null;
            _dbContext.SaveChanges();
        }

        public IEnumerable<int> GetYears(Guid clubId)
        {
            return
                _dbContext.Fines.Where(c => c.Rate.ClubId == clubId)
                    .Select(c => c.Issued.Year)
                    .ToList()
                    .Distinct()
                    .OrderByDescending(y => y);
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

        public int GetDueAmount(Guid memberId)
        {
            return _dbContext.Fines.Where(f => f.Paid == null && f.MemberId == memberId).Sum(f => f.Amount);
        }
    }
}
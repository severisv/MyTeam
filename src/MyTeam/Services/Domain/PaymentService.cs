using System;
using System.Collections.Generic;
using System.Linq;
using MyTeam.Models.Domain;
using MyTeam.Models;
using MyTeam.ViewModels.Payment;

namespace MyTeam.Services.Domain
{
    class PaymentService : IPaymentService
    {
        private readonly ApplicationDbContext _dbContext;

        public PaymentService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Delete(Guid paymentId)
        {
            var payment = _dbContext.Payments.Single(r => r.Id == paymentId);
            _dbContext.Payments.Remove(payment);
            _dbContext.SaveChanges();
        }

        public Guid Add(AddPaymentViewModel model, Guid clubId)
        {
            var payment = new Payment
            {
                Id = Guid.NewGuid(),
                ClubId = clubId,
                Amount = model.Amount.Value,
                Comment = model.Comment,
                MemberId = model.MemberId.Value,
                TimeStamp = model.Date ?? DateTime.Now,
            };
            _dbContext.Add(payment);
            _dbContext.SaveChanges();
            return payment.Id;
        }

        public PaymentViewModel Get(Guid id)
        {
            var query = _dbContext.Payments.Where(r => r.Id == id);

            return Select(query).Single();
        }

        public IEnumerable<PaymentViewModel> GetPayments(Guid clubId, int? year = null , Guid? memberId = null)
        {
            var query = _dbContext.Payments.Where(f => f.ClubId == clubId);

            if (year != null)
                query = query.Where(p => p.TimeStamp.Year == year);

            if (memberId != null) query = query.Where(f => f.MemberId == memberId);

            return Select(query).OrderByDescending(f => f.TimeStamp);

        }

        private static List<PaymentViewModel> Select(IQueryable<Payment> query)
        {
            return query.Select(f => new PaymentViewModel
            {
                MemberId = f.MemberId,
                Id = f.Id,
                Name = f.Member.Name,
                TimeStamp = f.TimeStamp,
                Amount = f.Amount,
                Comment = f.Comment,
                FirstName = f.Member.FirstName,
                LastName = f.Member.LastName,
                MemberImage = f.Member.Image,
                FacebookId = f.Member.FacebookId
            }).ToList();

        }


        public IEnumerable<int> GetYears(Guid clubId)
            =>
                _dbContext.Payments.Where(c => c.ClubId == clubId)
                    .Select(c => c.TimeStamp.Year)
                    .ToList()
                    .Distinct()
                    .OrderByDescending(y => y);
        

    }
}
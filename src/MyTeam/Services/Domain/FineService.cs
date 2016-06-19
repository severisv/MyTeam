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


        public void UpdateRate(RemedyRateViewModel model)
        {
            var rate = _dbContext.RemedyRates.Single(r => r.Id == model.Id);

            rate.Name = model.Name;
            rate.Description = model.Description;
            rate.Rate = model.Rate.Value;
            _dbContext.SaveChanges();
        }

        public void DeleteRate(Guid rateId)
        {
            var rate = _dbContext.RemedyRates.Single(r => r.Id == rateId);
            rate.IsDeleted = true;
            _dbContext.SaveChanges();
        }

        public void AddRate(Guid clubId, RemedyRateViewModel model)
        {
            var rate = new RemedyRate
            {
                Id = model.Id,
                ClubId = clubId,
                Description = model.Description,
                Name = model.Name,
                Rate = model.Rate.Value
            };
            _dbContext.Add(rate);
            _dbContext.SaveChanges();
        }

        public RemedyRateViewModel GetRate(Guid rateId)
        {
            var query = _dbContext.RemedyRates.Where(r => r.Id == rateId);

            return Select(query).Single();
        }

        public IEnumerable<RemedyRateViewModel> GetRates(Guid clubId)
        {
            var query = _dbContext.RemedyRates.Where(r => r.ClubId == clubId && !r.IsDeleted);

            return Select(query);
        }

        private static List<RemedyRateViewModel> Select(IQueryable<RemedyRate> query)
        {
            return query
                .Select(r => new RemedyRateViewModel
                {
                    Description = r.Description,
                    Rate = r.Rate,
                    Id = r.Id,
                    Name = r.Name
                })
                .ToList();
        }
    }
}
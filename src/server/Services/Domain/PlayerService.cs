using System;
using System.Linq;
using MyTeam.Models;

using MyTeam.Services.Application;

namespace MyTeam.Services.Domain
{

    class PlayerService : IPlayerService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ICacheHelper _cacheHelper;

        public PlayerService(ApplicationDbContext dbContext, ICacheHelper cacheHelper)
        {
            _cacheHelper = cacheHelper;
            _dbContext = dbContext;
        }  
       

        public void AddEmailToPlayer(string facebookId, string email)
        {
            if (string.IsNullOrWhiteSpace(facebookId)) return;
            var players = _dbContext.Members.Where(p => p.FacebookId == facebookId).ToList();
            foreach (var player in players)
            {
                if (string.IsNullOrWhiteSpace(player.UserName))
                {
                    player.UserName = email;
                }
            }
            _dbContext.SaveChanges();
        }      
  
    }

}
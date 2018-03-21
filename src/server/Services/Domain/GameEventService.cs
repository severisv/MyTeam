using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MyTeam.Models;
using MyTeam.Models.Domain;
using MyTeam.Models.Enums;
using MyTeam.ViewModels.Game;

namespace MyTeam.Services.Domain
{
    class GameEventService : IGameEventService
    {

        private readonly ApplicationDbContext _dbContext;

        public GameEventService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

      
        public GameEventViewModel AddGameEvent(GameEventViewModel model)
        {
            var assistedById = model.Type != GameEventType.Goal ? null : model.AssistedById;

            var gameEventId = Guid.NewGuid(); 
            _dbContext.Add(new GameEvent
            {
                Id = gameEventId,
                PlayerId = model.PlayerId,
                AssistedById = assistedById,
                GameId = model.GameId,
                CreatedDate = DateTime.Now,
                Type = model.Type,
            });
            _dbContext.SaveChanges();

            return new GameEventViewModel
            {
                Id = gameEventId,
                GameId = model.GameId,
                PlayerId = model.PlayerId,
                AssistedById = assistedById,
                Type = model.Type
            };
        }

        public void Delete(Guid eventId)
        {
            var gameEvent = _dbContext.GameEvents.Single(g => g.Id == eventId);
            _dbContext.Remove(gameEvent);
            _dbContext.SaveChanges();
        }
    }
}
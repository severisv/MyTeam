using System;
using System.Collections.Generic;
using MyTeam.ViewModels.Game;

namespace MyTeam.Services.Domain
{
    public interface IGameEventService
    {
        GameEventViewModel AddGameEvent(GameEventViewModel model);
        IEnumerable<GameEventViewModel> GetGameEvents(Guid gameId);
        void Delete(Guid eventId);
    }
}
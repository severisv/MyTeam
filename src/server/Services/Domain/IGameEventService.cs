using System;
using System.Collections.Generic;
using MyTeam.ViewModels.Game;

namespace MyTeam.Services.Domain
{
    public interface IGameEventService
    {
        GameEventViewModel AddGameEvent(GameEventViewModel model);
        void Delete(Guid eventId);
    }
}
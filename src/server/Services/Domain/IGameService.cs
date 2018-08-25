using System;
using System.Collections.Generic;
using MyTeam.ViewModels.Game;

namespace MyTeam.Services.Domain
{
    public interface IGameService
    {
        RegisterSquadEventViewModel GetRegisterSquadEventViewModel(Guid eventId);
        void PublishSquad(Guid eventId);
        GameViewModel GetGame(Guid gameId);
    }
}
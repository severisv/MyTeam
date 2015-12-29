using System;
using MyTeam.ViewModels.Events;
using MyTeam.ViewModels.Game;

namespace MyTeam.Services.Domain
{
    public interface IGameService
    {
        void SelectPlayer(Guid eventId, Guid playerId, bool isSelected);
        RegisterSquadEventViewModel GetRegisterSquadEventViewModel(Guid eventId);
    }
}
using System;
using System.Collections.Generic;
using MyTeam.ViewModels.Events;
using MyTeam.ViewModels.Game;

namespace MyTeam.Services.Domain
{
    public interface IGameService
    {
        void SelectPlayer(Guid eventId, Guid playerId, bool isSelected);
        RegisterSquadEventViewModel GetRegisterSquadEventViewModel(Guid eventId);
        void PublishSquad(Guid eventId);
        IEnumerable<GameViewModel> GetGames(Guid teamId, int year);
        IEnumerable<SeasonViewModel> GetSeasons(Guid teamId);
    }
}
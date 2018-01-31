using System;
using System.Collections.Generic;
using MyTeam.ViewModels.Game;

namespace MyTeam.Services.Domain
{
    public interface IGameService
    {
        void SelectPlayer(Guid eventId, Guid playerId, bool isSelected);
        RegisterSquadEventViewModel GetRegisterSquadEventViewModel(Guid eventId);
        void PublishSquad(Guid eventId);
        IEnumerable<GameViewModel> GetGames(Guid teamId, int year, string teamName);
        IEnumerable<SeasonViewModel> GetSeasons(Guid teamId);
        GameViewModel GetGame(Guid gameId);
        IEnumerable<PlayerViewModel> GetSquad(Guid gameId);
        void AddGames(List<ParsedGame> games, Guid clubId);
        string GetGamePlan(Guid gameId);
        void SaveGamePlan(Guid gameId, string gamePlan);
        void PublishGamePlan(Guid gameId);
    }
}
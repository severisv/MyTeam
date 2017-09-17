using System;
using System.Collections.Generic;
using MyTeam.Models.Dto;
using MyTeam.Models.Enums;
using MyTeam.Models.Structs;
using MyTeam.ViewModels.Player;

namespace MyTeam.Services.Domain
{
    public interface IPlayerService
    {
        JsonResponseMessage Add(string clubId, string facebookId, string firstName, string middleName, string lastName, string emailAddress);
        IEnumerable<string> GetFacebookIds();
        void SetPlayerStatus(Guid id, PlayerStatus status, Guid clubId);
        void TogglePlayerRole(Guid id, string role, Guid clubId);
        void EditPlayer(EditPlayerViewModel model, Guid clubId);
        void AddEmailToPlayer(string facebookId, string email);
        IEnumerable<SimplePlayerDto> GetDto(Guid clubId, PlayerStatus? status = null, bool includeCoaches = false);
        void TogglePlayerTeam(Guid teamId, Guid guid, Guid clubId);
        ShowPlayerViewModel GetSingle(Guid clubId, string urlName);
        ShowPlayerViewModel GetSingle(Guid playerId);
        IEnumerable<ListPlayerViewModel> GetPlayers(PlayerStatus status, Guid clubId);
        IEnumerable<PlayerStatsViewModel> GetStats(Guid playerId, IEnumerable<Guid> teamIds);
    }
}
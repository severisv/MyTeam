using System;
using System.Collections;
using System.Collections.Generic;
using MyTeam.Models.Domain;
using MyTeam.Models.Dto;
using MyTeam.Models.Enums;
using MyTeam.Models.Structs;
using MyTeam.ViewModels.Player;

namespace MyTeam.Services.Domain
{
    public interface IPlayerService
    {
        JsonResponseMessage Add(string clubId, string facebookId, string firstName,string middleName, string lastName, string emailAddress, string imageSmall, string imageMedium, string imageLarge);
        IEnumerable<string> GetFacebookIds();
        void SetPlayerStatus(Guid id, PlayerStatus status, string clubName);
        void TogglePlayerRole(Guid id, string role, string clubName);
        void EditPlayer(EditPlayerViewModel model, string clubId);
        void AddEmailToPlayer(string facebookId, string email);
        IEnumerable<SimplePlayerDto> GetDto(string clubId);
        void TogglePlayerTeam(Guid teamId, Guid guid, string clubName);
        ShowPlayerViewModel GetSingle(Guid playerId);
        IEnumerable<ShowPlayerViewModel> GetPlayers(PlayerStatus status, Guid clubId);
    }
}
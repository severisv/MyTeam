using System;
using System.Collections.Generic;
using MyTeam.Models.Enums;
using MyTeam.ViewModels.Player;

namespace MyTeam.Services.Domain
{
    public interface IPlayerService
    {
        void EditPlayer(EditPlayerViewModel model, Guid clubId);
        void AddEmailToPlayer(string facebookId, string email);
        ShowPlayerViewModel GetSingle(Guid playerId);
        IEnumerable<ListPlayerViewModel> GetPlayers(PlayerStatus status, Guid clubId);
    }
}
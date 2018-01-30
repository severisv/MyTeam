﻿using System;
using System.Collections.Generic;
using MyTeam.Models.Dto;
using MyTeam.Models.Enums;
using MyTeam.Models.Structs;
using MyTeam.ViewModels.Player;

namespace MyTeam.Services.Domain
{
    public interface IPlayerService
    {
        void EditPlayer(EditPlayerViewModel model, Guid clubId);
        void AddEmailToPlayer(string facebookId, string email);
        IEnumerable<SimplePlayerDto> GetDto(Guid clubId, PlayerStatus? status = null, bool includeCoaches = false);
        ShowPlayerViewModel GetSingle(Guid clubId, string urlName);
        ShowPlayerViewModel GetSingle(Guid playerId);
        IEnumerable<ListPlayerViewModel> GetPlayers(PlayerStatus status, Guid clubId);
        IEnumerable<PlayerStatsViewModel> GetStats(Guid playerId, IEnumerable<Guid> teamIds);
    }
}
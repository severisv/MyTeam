﻿using System;
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
        IEnumerable<Object> Get(string clubId);
        void SetPlayerStatus(Guid id, PlayerStatus status);
        void TogglePlayerRole(Guid id, string role);
        void EditPlayer(EditPlayerViewModel model);
        void AddEmailToPlayer(string facebookId, string email);
        IEnumerable<SimplePlayerDto> GetDto(string clubId);
    }
}
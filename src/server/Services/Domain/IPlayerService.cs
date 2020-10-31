using System;
using System.Collections.Generic;
using MyTeam.Models.Enums;

namespace MyTeam.Services.Domain
{
    public interface IPlayerService
    {
        void AddEmailToPlayer(string facebookId, string email);
    }
}
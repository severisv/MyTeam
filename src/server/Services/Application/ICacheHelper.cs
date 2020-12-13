using System;
using System.Collections.Generic;
using MyTeam.Models.Dto;

namespace MyTeam.Services.Application
{

    public interface ICacheHelper
    {
        PlayerDto GetPlayerFromUser(string name, Guid clubId);
        ClubDto GetCurrentClub(string clubId);
        void ClearCache(Guid? clubId, string email);
    }
}
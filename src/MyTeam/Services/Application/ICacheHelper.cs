using System;
using System.Collections.Generic;
using MyTeam.Models.Dto;

namespace MyTeam.Services.Application
{

    public interface ICacheHelper
    {
        PlayerDto GetPlayerFromUser(string name, Guid clubId);
        ClubDto GetCurrentClub(string clubId);
        void ClearCache(string clubId, string email);
        MemberNotification GetNotifications(Guid memberId, Guid clubId, IEnumerable<Guid> teamIds);
        void ClearNotificationCache(Guid clubId);
        void ClearNotificationCacheByMemberId(Guid clubId, Guid memberId);
        void ClearMemberCache(Guid memberId);
    }
}
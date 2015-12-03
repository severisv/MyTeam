using System;
using MyTeam.Models.Domain;

namespace MyTeam.Services.Domain
{
    public interface ITableService
    {
        Table GetTable(Guid seasonId);
        void Create(Guid seasonId, string tableString);
        void CreateSeason(Guid teamId, int year, string name);
    }
}
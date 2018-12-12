using System;

namespace MyTeam.Services.Domain
{
    public interface ITableService
    {
        void Update(Guid seasonId, string tableString);
    }
}
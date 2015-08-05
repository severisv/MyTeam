using System;
using System.Linq;
using MyTeam.Models.Domain;
using MyTeam.Services.Repositories;

namespace MyTeam.Services.Domain
{
    class TableService : ITableService
    {
        private readonly IRepository<Table> _tableRepository;

        public TableService(IRepository<Table> tableRepository)
        {
            _tableRepository = tableRepository;
        }


        public Table GetTable(Guid seasonId)
        {
            return _tableRepository.Get().Where(t => t.SeasonId == seasonId).OrderByDescending(t => t.CreatedDate).FirstOrDefault();
        }

        public void Create(Guid seasonId, string tableString)
        {
            var table = new Table(seasonId, tableString);
            _tableRepository.Add(table);
        }
    }
}
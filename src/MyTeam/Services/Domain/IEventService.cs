using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MyTeam.Models.Domain;

namespace MyTeam.Services.Domain
{
    public interface IEventService<TType> where TType : Event
    {
        TType Get(Guid id);
        IEnumerable<TType> GetUpcoming();
        IList<TType> GetAll();
        IEnumerable<TType> GetPrevious();
    }
}

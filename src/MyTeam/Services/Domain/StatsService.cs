using System;
using System.Collections.Generic;
using System.Linq;
using MyTeam.Models.Domain;
using MyTeam.Models.Enums;
using MyTeam.Services.Repositories;

namespace MyTeam.Services.Domain
{
    class StatsService : IStatsService
    {
        private readonly IRepository<Event> _eventRepository;

        public StatsService(IRepository<Event> eventRepository)
        {
            _eventRepository = eventRepository;
        }

        public IEnumerable<EventAttendance> GetAttendance(Guid clubId, int year)
        {
            return _eventRepository.Get()
                .Where(e => e.ClubId == clubId &&
                            e.DateTime.Year == year &&
                            (e.Type == EventType.Trening || e.Type == EventType.Kamp)).SelectMany(e => e.Attendees);

        }

        public IEnumerable<int> GetAttendanceYears(Guid clubId)
        {
            return _eventRepository.Get()
                .Where(e => e.ClubId == clubId && (e.Type == EventType.Trening || e.Type == EventType.Kamp))
                .Select(e => e.DateTime.Year).Distinct();
        }
    }
}
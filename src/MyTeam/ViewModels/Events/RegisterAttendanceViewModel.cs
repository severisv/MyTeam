using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Mvc;
using Microsoft.Data.Entity.Metadata;
using MyTeam.Models.Domain;
using MyTeam.Models.Dto;
using MyTeam.Models.Enums;
using MyTeam.Resources;

namespace MyTeam.ViewModels.Events
{
    public class RegisterAttendanceViewModel
    {
        public IEnumerable<EventViewModel>  PreviousEvents { get; } 
        public Training Training { get; }
        private readonly IEnumerable<RegisterAttendanceViewModel.PlayerAttendanceViewModel> _players;

        public IEnumerable<PlayerAttendanceViewModel> Attendees => _players.Where(p => Training.Attendees.Any(a => a.MemberId == p.Id && a.IsAttending));

        public IEnumerable<PlayerAttendanceViewModel> OtherActivePlayers => _players.Where(p => p.Status == PlayerStatus.Aktiv).Where(p => Attendees.All(a => a.Id != p.Id));

        public IEnumerable<PlayerAttendanceViewModel> OtherInactivePlayers => _players.Where(p => Attendees.All(a => a.Id != p.Id)).Where(p => OtherActivePlayers.All(a => a.Id != p.Id));

        public RegisterAttendanceViewModel(Training training, IEnumerable<SimplePlayerDto> players, IEnumerable<EventViewModel> previousEvents)
        {
            Training = training;
            _players = players.Select(p => new PlayerAttendanceViewModel(p, Training.Id,
                    training.Attendees.FirstOrDefault(a => a.MemberId == p.Id)
                ));
          
            PreviousEvents = previousEvents;
        }

        public class PlayerAttendanceViewModel : SimplePlayerDto
        {
            public EventAttendance Attendance { get; }
            public Guid EventId { get; }

            public PlayerAttendanceViewModel(SimplePlayerDto player, Guid eventId, EventAttendance attendance)
            {
                Attendance = attendance;
                Id = player.Id;
                Name = player.Name;
                ImageSmall = player.ImageSmall;
                Status = player.Status;
                EventId = eventId;
            }
        }
    }
}
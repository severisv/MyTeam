﻿using System.Collections.Generic;
using System.Linq;
using MyTeam.Models.Domain;
using MyTeam.Models.Enums;

namespace MyTeam.ViewModels.Attendance
{
    public class AttendanceViewModel
    {
        public IEnumerable<int> Years { get; set; }

        public IEnumerable<PlayerAttendanceViewModel> Players
        {
            get
            {
                var players = _attendance.GroupBy(attendance => attendance.MemberId).Select(group => group.First().Member);
                return players.Select(player => new PlayerAttendanceViewModel
                {
                    PlayerId = player.Id,
                    Name = player.Name,
                    Trainings = _attendance.Where(a => a.DidAttend && a.MemberId == player.Id).Count(a => a.EventType == EventType.Trening),
                    Games = _attendance.Where(a => a.DidAttend && a.MemberId == player.Id).Count(a => a.EventType == EventType.Kamp),
                    NoShows = _attendance.Where(a => a.IsAttending && !a.DidAttend && a.MemberId == player.Id).Count(a => a.EventType == EventType.Kamp),
                    ImageSmall =  player.ImageSmall
                }).ToList().OrderByDescending(p => p.Trainings).ThenBy(p => p.Games);
            }
        }

        public int Year { get; set; }
        
        private readonly IEnumerable<EventAttendanceViewModel> _attendance;


        public AttendanceViewModel(IEnumerable<EventAttendanceViewModel> attendance, IEnumerable<int> years, int year)
        {
            Year = year;
            Years = years;
            _attendance = attendance;
        }

     
      
    }
}
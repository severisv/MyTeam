using System;
using System.Collections.Generic;

namespace MyTeam.Models.Dto
{
    public class MemberNotification
    {
        public int UnansweredEvents { get; set; }
        public IEnumerable<Guid> UnansweredEventIds { get; set; }
        public bool Any => UnansweredEvents > 0;

    }
}
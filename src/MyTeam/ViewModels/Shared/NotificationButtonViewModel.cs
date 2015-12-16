using System;

namespace MyTeam.ViewModels.Shared
{
    public class NotificationButtonViewModel
    {
        public int UnansweredEvents { get; set; }
        public bool Any => UnansweredEvents > 0;
    }
}
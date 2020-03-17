using System;
using System.Collections.Generic;

namespace TrackManagement
{
    public abstract class Slot
    {
        public TimeSpan StartTime { get; set; }
        
        public TimeSpan EndTime { get; set; }

        public string Title { get; set; }
    }


    public class TalkSession : Slot
    {
        public List<Subject> Talks { get; set; }

        public int _duration;
        public int Duration
        {
            get { return _duration; }
            private set { _duration = (int) EndTime.Subtract(StartTime).TotalMinutes; }
        }

        public TimeSpan TimeRemaining { get; set; }

        public string Title { get; set; }
    }

    public class SessionSharingEvent : Slot
    {
        public TimeSpan StartTimeFrom { get; set; }

        public TimeSpan StartTimeTo { get; set; }
    }

    public class Break : Slot
    {
        public Break()
        {

        }
        
    }
}
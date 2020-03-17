using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace TrackManagement
{
    public interface IScheduler
    {
        void Schedule(IEnumerable<Day> days, IEnumerable<Subject> talks);
    }


    public class SimpleScheduler : IScheduler
    {
        List<Day> _days;
        private List<Subject> _subjects;

        public SimpleScheduler()
        {
            _days = new List<Day>();
            _subjects = new List<Subject>();
        }

        public void Schedule(IEnumerable<Day> days, IEnumerable<Subject> talks)
        {
            _days = days.ToList();
            _subjects = talks.ToList();

            SortSubjects();
            InitializeTracks();

            foreach (var talk in _subjects)
            {
                foreach (var day in _days)
                {
                    var isScheduledInMorning = ScheduleInMorning(talk, day);

                    if (!isScheduledInMorning)
                    {
                        ScheduleInEvening(talk, day);
                    }
                    ScheduleNetworkingEvent(day);
                }


            }
        }

        private void ScheduleNetworkingEvent(Day day)
        {
            foreach (var track in day.Tracks)
                track.SessionSharingEvent.StartTime = track.EveningSession.EndTime.Subtract(track.EveningSession.TimeRemaining);
        }

        private void InitializeTracks()
        {
            foreach (var day in _days)
            {
                foreach (var track in day.Tracks)
                {
                    track.MorningSession.Talks = new List<Subject>();
                    track.MorningSession.TimeRemaining =
                        track.MorningSession.EndTime.Subtract(track.MorningSession.StartTime);

                    track.EveningSession.Talks = new List<Subject>();
                    track.EveningSession.TimeRemaining =
                        track.EveningSession.EndTime.Subtract(track.EveningSession.StartTime);
                }
            }
        }

        private bool ScheduleInMorning(Subject talk, Day day)
        {
            foreach (var track in day.Tracks)
            {
                var duration = talk.Duration.Value * (int)(talk.Duration.Unit);
                if (TalkCanBeScheduledInMorning(duration, track))
                {
                    track.MorningSession.Talks.Add(talk);
                    track.MorningSession.TimeRemaining = track.MorningSession
                                                            .TimeRemaining.Subtract(new TimeSpan(0, duration, 0));
                    return true;
                }
            }
            return false;
        }

        private bool TalkCanBeScheduledInMorning(int duration, Track track)
        {
            return (duration <= track.MorningSession.TimeRemaining.TotalMinutes);
        }

        private bool ScheduleInEvening(Subject talk, Day day)
        {
            foreach (var track in day.Tracks)
            {
                var duration = talk.Duration.Value * (int)(talk.Duration.Unit);
                if (TalkCanBeScheduledInEvening(duration, track))
                {
                    track.EveningSession.Talks.Add(talk);
                    track.EveningSession.TimeRemaining = track.EveningSession
                                                              .TimeRemaining.Subtract(new TimeSpan(0, duration, 0));
                    return true;
                }
            }
            return false;
        }

        private bool TalkCanBeScheduledInEvening(int duration, Track track)
        {
            return (duration <= track.EveningSession.TimeRemaining.TotalMinutes);
        }

        private void SortSubjects()
        {
            _subjects = _subjects.OrderByDescending(t => (t.Duration.Value * (int)(t.Duration.Unit))).ToList();
        }


    }
}
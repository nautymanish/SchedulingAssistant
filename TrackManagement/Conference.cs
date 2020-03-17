using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace TrackManagement
{
    public class Conference
    {
        public List<Subject> SelectedSubjects { get; private set; }

        public ISubjectLoader SubjectsLoader { get; set; }

        public IScheduler Scheduler { get;private set; }

        public IResultFormatter ResultFormatter { get; set; } 

        public List<Day> Days { get; private set; }
        
        public int TotalSubjects {
            get
            {
                return SelectedSubjects.Count;
            }
        }

        private int _remainingTime;

        public Conference(IScheduler scheduler,IEnumerable<Day> days)
        {
            Days = new List<Day>();
            SelectedSubjects = new List<Subject>();

            Days = days.ToList();
            Scheduler = scheduler;
            CalculateRemainingTime();
        }
        
        public void Schedule()
        {
            Scheduler.Schedule(Days,SelectedSubjects);
        }
        
        public void GetSchedule()
        {
            ResultFormatter.Format(Days);
        }

        public void RegisterTalks()
        {
            
            try
            {
                var newSubjects = SubjectsLoader.Load();
                
                //if (CannotBeRegistered(newSubjects)) // advance code to check the limits
                //    throw new ArgumentException("Exceeding Time Limit");
                
                SelectedSubjects.InsertRange(SelectedSubjects.Count, newSubjects);
                
            }
            catch (ArgumentException e)
            {
                throw;
            }
           
        }

        public Subject GetSubjectByName(string topic)
        {
            return SelectedSubjects.FirstOrDefault(talk => string.Equals(talk.Topic, topic, StringComparison.OrdinalIgnoreCase));
        }

        private bool CannotBeRegistered(IEnumerable<Subject> newTalks)
        {
            var timeTaken = newTalks.Sum(newSubject => newSubject.Duration.Value*(int) newSubject.Duration.Unit);
            if (timeTaken > _remainingTime)
                return true;
            _remainingTime = _remainingTime- timeTaken;
            return false;
        }

        private void CalculateRemainingTime()
        {
            foreach (var day in Days)
            {
                foreach (var track in day.Tracks)
                {
                    _remainingTime +=
                        (int) track.MorningSession.EndTime.Subtract(track.MorningSession.StartTime).TotalMinutes;
                    _remainingTime +=
                        (int) track.EveningSession.EndTime.Subtract(track.EveningSession.StartTime).TotalMinutes;
                }
            }
        }

    }


    public class Track
    {
        public string Title { get; private set; }

        public TalkSession MorningSession { get; set; }

        public Break LunchBreak { get; set ; }

        public TalkSession EveningSession { get; set; }

        public SessionSharingEvent SessionSharingEvent { get; set; }
    }

    public class Day
    {
        public IEnumerable<Track> Tracks { get; private set; }

        public Day(IEnumerable<Track> tracks)
        {
            Tracks = tracks;
        }
    }
}


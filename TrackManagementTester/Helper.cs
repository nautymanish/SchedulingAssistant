using System;
using System.Collections.Generic;
using TrackManagement;

namespace TrackManagementTester
{
    public static class Helper
    {
        public static List<string> GetTalksListOne()
        {
            return new List<string>()
            {
                "A 45min",
                "Woah 30min",
                "B 30min",
                "C vs Noise 45min",
                "Rails Magic 60min",
                "Ruby on Rails: Why We Should Move On 60min",
                "Writing Fast Tests Against Enterprise Rails 60min",
                "Clojure Ate Scala (on my project) 45min",
                "Programming in the Boondocks of Seattle 30min",
                "Ruby vs. Clojure for Back-End Development 30min",
                "Ruby on Rails Legacy App Maintenance 60min",
                "A World Without HackerNews 30min",
                "User Interface CSS in Rails Apps 30min"
            };

        }

        public static List<string> GetTalksListTwo()
        {
            return new List<string>()
            {
                "Overdoing it in Python 45min",
                "Lua for the Masses 30min",
                "Ruby Errors from Mismatched Gem Versions 45min",
                "Common Ruby Errors 45min",
                "Rails for Python Developers 30min",
                "Communicating Over Distance 60min"

            };

        }

        public static List<string> GetInvalidTalksList()
        {
            return new List<string>()
            {
                "Overdoing it in Python 45min",
                "Lua for the Masses 30hours"
            };
        }

        public static Track GetNewTrack()
        {
            return new Track()
            {
                MorningSession = new TalkSession()
                {
                    Title = "Morning Session",
                    StartTime = new TimeSpan(09, 00, 00),
                    EndTime = new TimeSpan(12, 00, 00)
                },
                EveningSession = new TalkSession()
                {
                    Title = "Evening Session",
                    StartTime = new TimeSpan(01, 00, 00),
                    EndTime = new TimeSpan(5, 00, 00)
                },
                SessionSharingEvent = new SessionSharingEvent()
                {
                    Title = "Sharing Session",
                    StartTimeFrom = new TimeSpan(04, 00, 00),
                    StartTimeTo = new TimeSpan(05, 00, 00)
                },
                LunchBreak = new Break()
                {
                    Title = "Lunch",
                    StartTime = new TimeSpan(12, 00, 00),
                    EndTime = new TimeSpan(1, 00, 00)
                }
            };
        }

        public static List<string> GetLightningTalksList()
        {
            return new List<string>()
            {
                "Accounting-Driven Development Lightining",
                "Woah lightining"
            };
        }


        public static Subject GetInvalidTalk()
        {
            return new Subject("Topic", new SubjectDuration(TimeUnit.Min, 245));
        }
        public static List<string> GetTalkListPerCode()
        {
            return new List<string>()
            {
                "Organising Parents for Academy Improvements  60min",
"Teaching Innovations in the Pipeline  45min",
"Teacher Computer Hacks  30min",
"Making Your Academy Beautiful  45min",
"Academy Tech Field Repair  45min",
"Sync Hard  Lightining",
"Unusual Recruiting  Lightining",
"Parent Teacher Conferences  60min",
"Managing Your Dire Allowance  45min",
"Customer Care  30min",
"AIMs – 'Managing Up'  30min",
"Dealing with Problem Teachers  45min",
"Hiring the Right Cook  60min",
"Government Policy Changes and Bridge  60min",
"Adjusting to Relocation  45min",
"Public Works in Your Community  30min",
"Talking To Parents About Billing  30min",
"So They Say You're a Devil Worshipper  60min",
"Two-Streams or Not Two-Streams  30min",
"Piped Water  30min"
            };
        }
    }
}
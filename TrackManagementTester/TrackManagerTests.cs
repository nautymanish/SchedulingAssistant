using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using TrackManagement;
using NUnit.Framework;

namespace TrackManagementTester
{
    [TestFixture]
    public class ConferenceTrackManagerShould
    {
        private Conference _testConference;
        private SubjectDuration _duration;
        private IScheduler _scheduler;
        private List<Track> _tracks;
        private List<Day> _days;

        [SetUp]
        public void Initialize()
        {
            _duration = new SubjectDuration(TimeUnit.Min, 60);
            _scheduler=new SimpleScheduler();
            SetDaysSchedule();
            _testConference = new Conference(_scheduler,_days);
            _testConference.ResultFormatter = new TextFileFormatter();
        }

        

        private void SetDaysSchedule()
        {
            _days=new List<Day>();
            
            _tracks=new List<Track>();
            for (var i = 0; i < 2; i++)
            {
                _tracks.Add(Helper.GetNewTrack());
            }

            _days.Add(new Day(_tracks));
        }

        
        [TearDown]
        public void CleanUp()
        {
            _testConference = null;
        }

        [Test]
        public void DoNothing()
        {
            Assert.IsTrue(true);
        }

        [Test]
        public void ReturnZeroIfNoSubjectsRegistered()
        {
            Assert.AreEqual(_testConference.TotalSubjects, 0);
        }

        [Test]
        public void ReturnOneIfOneSubjectRegistered()
        {

            Subject testTalk = new Subject("test Topic", _duration);

            _testConference.SubjectsLoader = new SingleSubjectLoader(testTalk);
            _testConference.RegisterTalks();

            Assert.AreEqual(_testConference.TotalSubjects, 1);
        }

        [Test]
        public void RegisterTheSubjectProperly()
        {
            Subject testSubject = new Subject("test Topic", _duration);

            _testConference.SubjectsLoader=new SingleSubjectLoader(testSubject);
            _testConference.RegisterTalks();
            Subject registerTestTalk = _testConference.GetSubjectByName("test Topic");

            Assert.AreEqual(testSubject.Topic, registerTestTalk.Topic);
            Assert.AreEqual(_testConference.TotalSubjects, 1);
            Assert.AreEqual(testSubject.Duration, registerTestTalk.Duration);
        }

        [Test]
        public void RegisterLightningTalks()
        {
            _testConference.SubjectsLoader = new FileSubjectsLoader(Helper.GetLightningTalksList());
            _testConference.RegisterTalks();
        }

        [Test]
        public void RegisterMultipleTalksProperly()
        {
            var testSubject1 = new Subject("TopicOne", _duration);
            var testSubject2 = new Subject("TopicTwo", _duration);
            var testSubject3 = new Subject("TopicThree", _duration);

            _testConference.SubjectsLoader = new SingleSubjectLoader(testSubject1);
            _testConference.RegisterTalks();
            _testConference.SubjectsLoader = new SingleSubjectLoader(testSubject2);
            _testConference.RegisterTalks();
            _testConference.SubjectsLoader = new SingleSubjectLoader(testSubject3);
            _testConference.RegisterTalks();
            

            var registeredSubject = _testConference.GetSubjectByName("TopicOne");

            Assert.AreEqual(_testConference.TotalSubjects, 3);
            Assert.AreEqual(testSubject1.Topic, registeredSubject.Topic);
        }

        [Test]
        public void ReturnNullIfTalkIsNotRegistered()
        {
            Subject testTalk1 = new Subject("TopicOne", _duration);
            _testConference.SubjectsLoader = new SingleSubjectLoader(testTalk1);
            _testConference.RegisterTalks();
            
            var registeredTalk = _testConference.GetSubjectByName("TopicTwo");

            Assert.AreEqual(_testConference.TotalSubjects, 1);
            Assert.AreEqual(null, registeredTalk);
        }

        [Test]
        [ExpectedException]
        public void NotRegisterTalksIfItCannotBeScheduled()
        {
            _testConference = new Conference(_scheduler, new List<Day>(){ new Day( new List<Track>(){
                                                                            Helper.GetNewTrack()
                                                                          })});
         
            _testConference.SubjectsLoader = new FileSubjectsLoader(Helper.GetTalksListOne());
            _testConference.RegisterTalks();

        }

        [Test]
        public void BeAbleToImportTalksList()
        {
            _testConference.SubjectsLoader = new FileSubjectsLoader(Helper.GetTalksListOne());
            _testConference.RegisterTalks();
            Assert.AreEqual(_testConference.TotalSubjects, 13);
        }

        [Test]
        public void AddTalksIfTheyWereAddedInTwoTurns()
        {
            _testConference.SubjectsLoader = new FileSubjectsLoader(Helper.GetTalksListOne());
            _testConference.RegisterTalks();

            _testConference.SubjectsLoader = new FileSubjectsLoader(Helper.GetTalksListTwo());
            _testConference.RegisterTalks();

            Assert.AreEqual(19, _testConference.TotalSubjects);
        }

        [Test]
        public void ScheduleTalks()
        {
            _testConference.SubjectsLoader = new FileSubjectsLoader(Helper.GetTalksListOne());
            _testConference.RegisterTalks();

            _testConference.Schedule();
        }

        [Test]
        public void AlsoScheduleTalksWithSharingEvent()
        {
            _testConference.SubjectsLoader = new FileSubjectsLoader(Helper.GetTalksListOne());
            _testConference.RegisterTalks();

            _testConference.SubjectsLoader = new FileSubjectsLoader(Helper.GetTalksListTwo());
            _testConference.RegisterTalks();

            _testConference.Schedule();
        }

        [Test]
        public void ScheduleAllTheTalksRegistered()
        {
            _testConference.SubjectsLoader = new FileSubjectsLoader(Helper.GetTalksListOne());
            _testConference.RegisterTalks();

            _testConference.Schedule();

            Assert.AreEqual(13, GetScheduledTalks());
        }

        [Test]
        public void ScheduleIfTalksWereRegisteredInIterations()
        {
            _testConference.SubjectsLoader = new FileSubjectsLoader(Helper.GetTalksListOne());
            _testConference.RegisterTalks();

            _testConference.SubjectsLoader = new FileSubjectsLoader(Helper.GetTalksListTwo());
            _testConference.RegisterTalks();

            _testConference.Schedule();
            Assert.AreEqual(19, GetScheduledTalks());
        }

        [Test]
        public void WriteResultToTextFile()
        {
            _testConference.SubjectsLoader = new FileSubjectsLoader(Helper.GetTalksListOne());
            _testConference.RegisterTalks();

            _testConference.SubjectsLoader = new FileSubjectsLoader(Helper.GetTalksListTwo());
            _testConference.RegisterTalks();

            _testConference.Schedule();

            _testConference.GetSchedule();
        }


        private int GetScheduledTalks()
        {
            return _testConference.Days.Sum(day => day.Tracks.Sum( track=>track.EveningSession.Talks.Count + track.MorningSession.Talks.Count));
        }
    }

    [TestFixture]
    public class TalkShould
    {
        private Subject _testTalk;
        private SubjectDuration _duration;
        [SetUp]
        public void Initialize()
        {
            _duration = new SubjectDuration(TimeUnit.Min, 60);
            _testTalk = new Subject("Topic", _duration);
        }

        [Test]
        [ExpectedException]
        public void ThrowExceptionIfTitleContainsNumbers()
        {
            _testTalk = new Subject("Topic1", _duration);
        }

        [Test]
        [ExpectedException]
        public void ThrowExceptionForInvalidDuration()
        {
            _duration = new SubjectDuration(TimeUnit.Min, -10);
            _testTalk = new Subject("Topic", _duration);
        }

    }

    [TestFixture]
    public class SchedulerShould
    {
        [Test]
        [ExpectedException]
        public void ThrowExceptionWhenNullTracksWerePassed()
        {
            var scheduler = new SimpleScheduler();
            scheduler.Schedule(null,null);
        }
    }
}

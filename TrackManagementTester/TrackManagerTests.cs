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

            Subject testSubject = new Subject("test Topic", _duration);

            _testConference.SubjectsLoader = new SingleSubjectLoader(testSubject);
            _testConference.RegisterSubjects();

            Assert.AreEqual(_testConference.TotalSubjects, 1);
        }

        [Test]
        public void RegisterTheSubjectProperly()
        {
            Subject testSubject = new Subject("test Topic", _duration);

            _testConference.SubjectsLoader=new SingleSubjectLoader(testSubject);
            _testConference.RegisterSubjects();
            Subject registerTestSubject = _testConference.GetSubjectByName("test Topic");

            Assert.AreEqual(testSubject.Topic, registerTestSubject.Topic);
            Assert.AreEqual(_testConference.TotalSubjects, 1);
            Assert.AreEqual(testSubject.Duration, registerTestSubject.Duration);
        }

        [Test]
        public void RegisterLightningSubjects()
        {
            _testConference.SubjectsLoader = new FileSubjectsLoader(Helper.GetLightningSubjectsList());
            _testConference.RegisterSubjects();
        }

        [Test]
        public void RegisterMultipleSubjectsProperly()
        {
            var testSubject1 = new Subject("TopicOne", _duration);
            var testSubject2 = new Subject("TopicTwo", _duration);
            var testSubject3 = new Subject("TopicThree", _duration);

            _testConference.SubjectsLoader = new SingleSubjectLoader(testSubject1);
            _testConference.RegisterSubjects();
            _testConference.SubjectsLoader = new SingleSubjectLoader(testSubject2);
            _testConference.RegisterSubjects();
            _testConference.SubjectsLoader = new SingleSubjectLoader(testSubject3);
            _testConference.RegisterSubjects();
            

            var registeredSubject = _testConference.GetSubjectByName("TopicOne");

            Assert.AreEqual(_testConference.TotalSubjects, 3);
            Assert.AreEqual(testSubject1.Topic, registeredSubject.Topic);
        }

        [Test]
        public void ReturnNullIfSubjectIsNotRegistered()
        {
            Subject testSubject1 = new Subject("TopicOne", _duration);
            _testConference.SubjectsLoader = new SingleSubjectLoader(testSubject1);
            _testConference.RegisterSubjects();
            
            var registeredSubject = _testConference.GetSubjectByName("TopicTwo");

            Assert.AreEqual(_testConference.TotalSubjects, 1);
            Assert.AreEqual(null, registeredSubject);
        }

        [Test]
        [ExpectedException]
        public void NotRegisterSubjectsIfItCannotBeScheduled()
        {
            _testConference = new Conference(_scheduler, new List<Day>(){ new Day( new List<Track>(){
                                                                            Helper.GetNewTrack()
                                                                          })});
         
            _testConference.SubjectsLoader = new FileSubjectsLoader(Helper.GetSubjectsListOne());
            _testConference.RegisterSubjects();

        }

        [Test]
        public void BeAbleToImportSubjectsList()
        {
            _testConference.SubjectsLoader = new FileSubjectsLoader(Helper.GetSubjectsListOne());
            _testConference.RegisterSubjects();
            Assert.AreEqual(_testConference.TotalSubjects, 13);
        }

        [Test]
        public void AddSubjectsIfTheyWereAddedInTwoTurns()
        {
            _testConference.SubjectsLoader = new FileSubjectsLoader(Helper.GetSubjectsListOne());
            _testConference.RegisterSubjects();

            _testConference.SubjectsLoader = new FileSubjectsLoader(Helper.GetSubjectsListTwo());
            _testConference.RegisterSubjects();

            Assert.AreEqual(19, _testConference.TotalSubjects);
        }

        [Test]
        public void ScheduleSubjects()
        {
            _testConference.SubjectsLoader = new FileSubjectsLoader(Helper.GetSubjectsListOne());
            _testConference.RegisterSubjects();

            _testConference.Schedule();
        }

        [Test]
        public void AlsoScheduleSubjectsWithSharingEvent()
        {
            _testConference.SubjectsLoader = new FileSubjectsLoader(Helper.GetSubjectsListOne());
            _testConference.RegisterSubjects();

            _testConference.SubjectsLoader = new FileSubjectsLoader(Helper.GetSubjectsListTwo());
            _testConference.RegisterSubjects();

            _testConference.Schedule();
        }

        [Test]
        public void ScheduleAllTheSubjectsRegistered()
        {
            _testConference.SubjectsLoader = new FileSubjectsLoader(Helper.GetSubjectsListOne());
            _testConference.RegisterSubjects();

            _testConference.Schedule();

            Assert.AreEqual(13, GetScheduledSubjects());
        }

        [Test]
        public void ScheduleIfSubjectsWereRegisteredInIterations()
        {
            _testConference.SubjectsLoader = new FileSubjectsLoader(Helper.GetSubjectsListOne());
            _testConference.RegisterSubjects();

            _testConference.SubjectsLoader = new FileSubjectsLoader(Helper.GetSubjectsListTwo());
            _testConference.RegisterSubjects();

            _testConference.Schedule();
            Assert.AreEqual(19, GetScheduledSubjects());
        }

        [Test]
        public void WriteResultToTextFile()
        {
            _testConference.SubjectsLoader = new FileSubjectsLoader(Helper.GetSubjectsListOne());
            _testConference.RegisterSubjects();

            _testConference.SubjectsLoader = new FileSubjectsLoader(Helper.GetSubjectsListTwo());
            _testConference.RegisterSubjects();

            _testConference.Schedule();

            _testConference.GetSchedule();
        }


        private int GetScheduledSubjects()
        {
            return _testConference.Days.Sum(day => day.Tracks.Sum( track=>track.EveningSession.Subjects.Count + track.MorningSession.Subjects.Count));
        }
    }

    [TestFixture]
    public class SubjectShould
    {
        private Subject _testSubject;
        private SubjectDuration _duration;
        [SetUp]
        public void Initialize()
        {
            _duration = new SubjectDuration(TimeUnit.Min, 60);
            _testSubject = new Subject("Topic", _duration);
        }

        [Test]
        [ExpectedException]
        public void ThrowExceptionIfTitleContainsNumbers()
        {
            _testSubject = new Subject("Topic1", _duration);
        }

        [Test]
        [ExpectedException]
        public void ThrowExceptionForInvalidDuration()
        {
            _duration = new SubjectDuration(TimeUnit.Min, -10);
            _testSubject = new Subject("Topic", _duration);
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

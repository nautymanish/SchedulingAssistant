using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace TrackManagement
{
    public interface ISubjectLoader
    {
        IEnumerable<Subject> Load();
    }
    
    public class FileSubjectsLoader : ISubjectLoader
    {
        public List<string> SubjectsList { get; private set; }

        public FileSubjectsLoader(List<string> subjectList)
        {
            SubjectsList = new List<string>();
            SubjectsList = subjectList;
        }

        public IEnumerable<Subject> Load()
        {
            var registeredSubjects = new List<Subject>();

            foreach (var subject in SubjectsList)
            {
                var unit = CheckForValidUnit(subject);
                string topic=null;
                

                var durationPosition = subject.IndexOfAny("0123456789".ToCharArray());

                var duration = "1";
                
                if (durationPosition == -1)
                    topic = subject.Substring(0, subject.LastIndexOf(unit, StringComparison.OrdinalIgnoreCase));
                else
                {
                    topic = subject.Substring(0, durationPosition);
                    
                    duration = subject.Substring(durationPosition,
                             subject.LastIndexOfAny("0123456789".ToCharArray()) - durationPosition + 1);
                }
                

                var Duration = new SubjectDuration((TimeUnit)Enum.Parse(typeof(TimeUnit), unit, true)
                                                    ,Convert.ToInt32(duration));

                registeredSubjects.Add(new Subject(topic, Duration));
            }
            
            return registeredSubjects;
        }

        private string CheckForValidUnit(string talk)
        {
            foreach (var unit in Enum.GetValues(typeof (TimeUnit)))
            {
                if (talk.IndexOf(unit.ToString(), StringComparison.OrdinalIgnoreCase) > -1)
                    return unit.ToString();
            }
            return null;
        }
    }

    public class SingleSubjectLoader : ISubjectLoader
    {
        private Subject _subject;

        public SingleSubjectLoader(Subject subject)
        {
            _subject = subject;
        }

        public IEnumerable<Subject> Load()
        {
            return new List<Subject>(){
                _subject
            };
        }
    }
}

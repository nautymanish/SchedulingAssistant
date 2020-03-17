# SchedulingAssistant
An example of how to schedule the courses for days with different tracks which can be chosen per convenience.

Problem statement is to have the possible schedule given for a particular day. This is a problem invovling sequencing and probably genetic algorithms for dynamic complexity which may arise. I had tried to place the code as linear and isolated as possible. The code starts with the following objects
a.Conference - this class is a repository to have details of all the talks(subjects) and the scheduler to manage the subject accrodingly
b.Scheduler - This is having a interface for Scheduling the tracks for a prticular day
        Internally had created a simple scheduler, per requirement other users can derive the same interface to create their flavours of scheduling
c.Slot- this is an abstract class and is being derived in to 3 childs, morning, lunch and evening slot, each having its own defined time limits
d. Subject & SubjectLoder  - Subject is a class having the real subjects with time duration for individual subjects, nd the SubjectLoader is an intermediatory to load dat from a data set like, list of strings etc.
e. ResultFormatter - Is a class to output dat in form of text file

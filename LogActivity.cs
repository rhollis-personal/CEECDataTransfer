using System;
using System.Collections.Generic;

namespace CeecDataTransfer
{
    public enum MessageType
    {
        Text = 0,
        Html = 1,
        Xml = 2,
        Json = 3       
    }

    public enum ActionType
    {
        NewEmployee = 1,
        UpdateEmployee = 2,
        UpdateEnrollment = 3,
        GetToken = 4,
        SetUpLog = 5,
        WriteToLogInfo = 6,
        WriteToLogError = 7,
        CourseShell = 8,
        ExecuteCompleted = 9,
        ManualEmployeeUpdate = 10
    }

    public class LogActivity
    {
        public int ApplicationId { get; set; }
        public string ApplicationName { get; set; }
        public List<Actions> Actions { get; set; }   
        public string NotificatonEmailAddresses { get; set; }

    }

    public class Actions
    {
        public  int ActionId { get; set; }
        public string Name { get; set; }
    }

    public class Scheduler
    {
        public int ApplicationId { get; set; }
        public string ApplicationName { get; set; }
        public bool IsRunning { get; set; }
        public string Mode { get; set; }
        public int IntervalMins { get; set;  }
        public DateTime ExecuteDateTime { get; set; }
        public int DaysBack { get; set; }
        public string NotificatonEmailAddresses { get; set; }
    }
}

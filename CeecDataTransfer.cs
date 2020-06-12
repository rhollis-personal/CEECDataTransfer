namespace CeecDataTransfer
{
    using System.ServiceProcess;
    using System.Timers;
    using System;
    using System.Configuration;
    using System.Collections.Generic;

    partial class CeecDataTransfer : ServiceBase
    {
        public const string ABSORB_LMS = "Absorb LMS";
        public const string CTEC_LMS = "CTec LMS";
        public const string RED_VECTOR_LMS = "Red Vector LMS";
        public const string EXTERNAL_LMS = "External LMS";
        //public const string LAWROOM_LMS = "Law Room LMS";
        public const string BENTLEY_LMS = "Bentley LMS";
        public const string USER_NAME = "UserNameEmail";
        public const string COURSE_SHELL = "CourseShellEmail";

        public CeecDataTransfer()
        {            
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            var _pollingInterval = ConfigurationManager.AppSettings["PollingIntervalInMinutes"];
            var _pollingIntervalMinutes = 0;
            var result = int.TryParse(_pollingInterval, out _pollingIntervalMinutes);
            var _timer = new Timer(60 * 1000 * _pollingIntervalMinutes);  // 1 minute expressed as milliseconds
            _timer.Elapsed += new ElapsedEventHandler(test);
            _timer.AutoReset = true;
            _timer.Start();
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
        }

        protected override void OnStop()
        {
            // TODO: Add code here to perform any tear-down necessary to stop your service.
        }

        private void test(object sender, ElapsedEventArgs e)
        {
            Carollo.WriteToLog("Starting course processing at " + DateTime.Now.ToString("HH:mm:ss"), MessageType.Text, "System", ActionType.SetUpLog.ToString(), true, DateTime.Now, "");

            List<Scheduler> Scheduler = new List<Scheduler>();
            Scheduler = Carollo.GetApplicationScheduler();

            if (Scheduler.Count > 0)
            {
                //Carollo.WriteToLog("Starting course processing at " + DateTime.Now.ToString("HH:mm:ss"), MessageType.Text, "System", ActionType.SetUpLog.ToString(), true, DateTime.Now, "");
                Carollo.GetEmployeeDataToAbsorb();

                foreach (var scheduler in Scheduler)
                {
                    var executeCourseProcessing = false;

                    // Fetch data for applications that process course data
                    switch (scheduler.ApplicationName)
                    {
                        // Ctec LMS
                        case CTEC_LMS:
                            {
                                Carollo.LoadCtecDataToDB(scheduler.DaysBack);
                                executeCourseProcessing = true;
                                break;
                            }

                        // Red Vector
                        case RED_VECTOR_LMS:
                            {
                                Carollo.LoadRedVectorDataToDB(scheduler.DaysBack);
                                executeCourseProcessing = true;
                                break;
                            }

                        //**************   07/2018 Removed as Lawroom courses are now embeded in CEEC
                        // LawRoom
                        //case LAWROOM_LMS:
                        //    {
                        //        // Loading of external data is a manual process
                        //        executeCourseProcessing = true;
                        //        break;
                        //    }
                        //*************************************************************************************
                        
                        // Bentley
                        case BENTLEY_LMS:
                            {
                                // Loading of external data is a manual process
                                executeCourseProcessing = true;
                                break;
                            }

                        case USER_NAME:
                            {
                                Email.SendUserNameChanges();
                                break;
                            }

                        case COURSE_SHELL:
                            {
                                Email.SendCourseShellNotices();
                                break;
                            }

                        default:
                            {
                                // TODO: Create other application processes as needed and add them to the above list
                                break;
                            }
                    }

                    if (executeCourseProcessing)
                    {
                        CourseProcessing.Process(scheduler.ApplicationName);
                    }

                    Carollo.UpdateApplicationScheduler(scheduler);
                    //Carollo.WriteToLog("Finished course processing at " + DateTime.Now.ToString("HH:mm:ss"), MessageType.Text, "System", ActionType.SetUpLog.ToString(), true, DateTime.Now, "");
                }
            }
            Carollo.WriteToLog("Finished course processing at " + DateTime.Now.ToString("HH:mm:ss"), MessageType.Text, "System", ActionType.SetUpLog.ToString(), true, DateTime.Now, "");
        }
    }
}

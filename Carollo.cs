using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using CeecDataTransfer.Models;

namespace CeecDataTransfer
{
    public class Carollo
    {       
        private const string CTECConnection = "CTECConnection";
        private const string ActivityConnection = "ActivityConnection";
        private const string DisciplineName = "ExceptionDisplineArea";
        private const string ArchivedCDPTables = "ArchivedCDPTables";
        private const string ArchivedConfigYear = "ArchivedConfigYear";
        public const string ABSORB_LMS = "Absorb LMS";

        public enum ProcessActions
        {
            Add = 1,
            Update = 2,
            Skip = 3
        }
        public static void UpdateApplicationScheduler(Scheduler scheduler)
        {
            using (SqlConnection conn = new SqlConnection(ReadConfigFile(ActivityConnection)))
            {
                SqlCommand command = new SqlCommand("asp_UpdateApplicationScheduler", conn);

                conn.Open();

                try
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@ApplicationId", SqlDbType.Int).Value = scheduler.ApplicationId;

                    command.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Config.LogError(e);
                }
            }
        }
        public static void UpdateUserNameChange(string HCMUserId)
        {
            using (SqlConnection conn = new SqlConnection(ReadConfigFile(ActivityConnection)))
            {
                using (SqlCommand command = new SqlCommand("asp_UpdateUserNameChange", conn))
                {
                    conn.Open();

                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@HCMUserId", SqlDbType.NVarChar).Value = HCMUserId;

                    command.ExecuteNonQuery();
                }
            }
        }
        public static List<Scheduler> GetApplicationScheduler()
        {
            var Scheduler = new List<Scheduler>();
            using (SqlConnection conn = new SqlConnection(ReadConfigFile(ActivityConnection)))
            {
                SqlCommand command = new SqlCommand("asp_GetApplicationScheduler", conn);
                conn.Open();

                try
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Execute the command and process the results.
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Scheduler.Add(new Scheduler
                            {
                                ApplicationId = Convert.ToInt32(reader["ApplicationsId"]),
                                ApplicationName = Convert.ToString(reader["Name"]),
                                IsRunning = Convert.ToBoolean(reader["IsRunning"]),
                                Mode = Convert.ToString(reader["Mode"]),
                                IntervalMins = Convert.ToInt16(reader["IntervalMins"]),
                                ExecuteDateTime = Convert.ToDateTime(reader["ExecuteDateTime"]),
                                DaysBack = Convert.ToInt16(reader["DaysBack"]),
                                NotificatonEmailAddresses = Convert.ToString(reader["NotificatonEmailAddresses"])
                            });                          
                        }
                    }
                }
                catch (Exception e)
                {
                    Config.LogError(e);
                }
            }

            return Scheduler;
        }
        public static bool IsLogSetUpForApp(List<LogActivity> LogActivity)
        {          
            foreach (var log in LogActivity)
            {
                if(log.ApplicationId > 0)
                {
                    return true;
                }
            }

            return false;
        }
        public static void WriteToLog(string Message, MessageType Type, string Application, string Action, bool IsNotify, DateTime startDate, string Vendor)
        {
            if (Action.ToString()=="7")
            {
                Email.SendErrors(Message, Application);
            }
            InsertActivity(Message, Type, Application, Action, IsNotify, startDate);
        }
        public static void WriteToLog(string Message, MessageType Type, string Application, string Action, bool IsNotify, DateTime startDate)
        {
            if (Action.ToString() == "7")
            {
                Email.SendErrors(Message, Application);
            }
            InsertActivity(Message, Type, Application, Action, IsNotify, startDate);
        }
        private static void InsertActivity(string Message, MessageType Type, string Application, string Action, bool IsNotify, DateTime startDate)
        {

            using (SqlConnection conn = new SqlConnection(ReadConfigFile(ActivityConnection)))
            {
                SqlCommand command = new SqlCommand("asp_InsertApplicationActivity", conn);
                conn.Open();

                try
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.Add("@ApplicationId", SqlDbType.Int).Value = 3;
                    command.Parameters.Add("@ActionId", SqlDbType.Int).Value = 0;
                    command.Parameters.Add("@MessageType", SqlDbType.Int).Value = Type;
                    command.Parameters.Add("@Message", SqlDbType.NVarChar).Value = Message;
                    command.Parameters.Add("@IsNotify", SqlDbType.Bit).Value = Convert.ToInt32(IsNotify);
                    command.Parameters.Add("@ApplicationName", SqlDbType.NVarChar).Value = Application;
                    command.Parameters.Add("@ActionName", SqlDbType.NVarChar).Value = Action;
                    command.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = startDate;
                    command.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = DateTime.Now;

                    command.ExecuteReader();
                }
                catch (Exception e)
                {
                    Config.LogError(e);
                }
            }

        }
        public static void WriteToLog(string message, MessageType type, LogActivity LogActivity, bool IsNotify,string UserId, string CourseId, string CourseName, string EmployeeId, string Vendor, string UpdatesMade)
        {
            InsertActivity(message, type, LogActivity, IsNotify, UserId, CourseId, CourseName, EmployeeId, (Vendor ?? String.Empty).ToString(), UpdatesMade);
        }
        private static void InsertActivity(string Message, MessageType Type, LogActivity LogActivity, bool IsNotify, string UserId, string CourseId, string CourseName, string EmployeeId, string Vendor, string UpdatesMade)
        {
            var ActionId = 0;
            foreach (var action in LogActivity.Actions)
            {
                ActionId = action.ActionId;
            }

            using (SqlConnection conn = new SqlConnection(ReadConfigFile(ActivityConnection)))
            {
                SqlCommand command = new SqlCommand("asp_InsertApplicationActivity", conn);

                conn.Open();

                try
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.Add("@ApplicationId", SqlDbType.Int).Value = LogActivity.ApplicationId;
                    command.Parameters.Add("@ActionId", SqlDbType.Int).Value = ActionId;
                    command.Parameters.Add("@MessageType", SqlDbType.Int).Value = Type;
                    command.Parameters.Add("@Message", SqlDbType.NVarChar).Value = Message;
                    command.Parameters.Add("@IsNotify", SqlDbType.Bit).Value = Convert.ToInt32(IsNotify);
                    command.Parameters.Add("@ApplicationName", SqlDbType.NVarChar).Value = "";
                    command.Parameters.Add("@ActionName", SqlDbType.NVarChar).Value = "";
                    command.Parameters.Add("@UserId", SqlDbType.NVarChar).Value = UserId;
                    command.Parameters.Add("@CourseId", SqlDbType.NVarChar).Value = CourseId;
                    command.Parameters.Add("@CourseName", SqlDbType.NVarChar).Value = CourseName;
                    command.Parameters.Add("@EmployeeID", SqlDbType.NVarChar).Value = EmployeeId;
                    command.Parameters.Add("@Vendor", SqlDbType.NVarChar).Value = Vendor;
                    command.Parameters.Add("@UpdatesMade", SqlDbType.NVarChar).Value = UpdatesMade;

                    command.ExecuteReader();
                }
                catch (Exception e)
                {
                    Config.LogError(e);
                }
            }
      }
        public static LogActivity DetermineAction(LogActivity LogActivity, string action)
        {            
            var results = new LogActivity()
            {
                Actions = new List<Actions>()              
            };
           
            var Action = new Actions();
            Action = LogActivity.Actions.Find(i => i.Name == action);
            
            results.ApplicationId = LogActivity.ApplicationId;
            results.ApplicationName = LogActivity.ApplicationName;
            
            results.Actions.Add(new Actions()
            {
                ActionId = Action.ActionId,
                Name = Action.Name
            });           

            return results;
        }
        public static List<LogActivity> GetApplicationActions(string applicationName)
        {
            var results = new LogActivity()
            {
                Actions = new List<Actions>()                
            };

            var LogActivity = new List<LogActivity>();           
            
            using (SqlConnection conn = new SqlConnection(ReadConfigFile(ActivityConnection)))
            {
                SqlCommand command = new SqlCommand("asp_GetApplicationAction", conn); 
                conn.Open();

                try
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@ApplicationName", SqlDbType.NVarChar).Value = applicationName;

                    // Execute the command and process the results.
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            results.ApplicationId = Convert.ToInt16(reader["ApplicationsId"]);
                            results.ApplicationName = Convert.ToString(reader["ApplicationName"]); 
                            results.NotificatonEmailAddresses = Convert.ToString(reader["NotificatonEmailAddresses"]);
                        }

                        if (reader.NextResult())
                        {
                            while (reader.Read())
                            {
                                results.Actions.Add(new Actions()
                                {
                                    ActionId = Convert.ToInt16(reader["ActionsId"]),
                                    Name = Convert.ToString(reader["Name"])
                                });
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Config.LogError(e);
                }
                finally
                {
                    LogActivity.Add(results);
                }
            }
            
            return LogActivity;
        }
        public static List<CourseCompletion> GetCourseCompletions(String Vendor)
        {
            var completions = new List<CourseCompletion>();

            using (SqlConnection conn = new SqlConnection(ReadConfigFile(ActivityConnection)))
            {
                SqlCommand command = new SqlCommand("asp_GetCourseCompletions", conn);

                try
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@Vendor", SqlDbType.NVarChar).Value = Vendor;
                    conn.Open();

                    // Execute the command and process the results.
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            completions = DataReaderMapToList<CourseCompletion>(reader);
                        }
                    }
                }
                catch (Exception e)
                {
                    Config.LogError(e);
                }
            }

            return completions;
        }
        public static List<CourseShells> GetMissingCourseShells()
        {
            var courseShells = new List<CourseShells>();

            using (SqlConnection conn = new SqlConnection(ReadConfigFile(ActivityConnection)))
            {
                SqlCommand command = new SqlCommand("asp_GetMissingCourseShellNotifications", conn);

                try
                {
                    command.CommandType = CommandType.StoredProcedure;
                    conn.Open();

                    // Execute the command and process the results.
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            courseShells = DataReaderMapToList<CourseShells>(reader);
                        }
                    }

                }
                catch (Exception e)
                {
                    Config.LogError(e);
                }
            }

            return courseShells;
        }
        public static List<NameChanges> GetUserNameChanges()
        {
            var nameChanges = new List<NameChanges>();

            using (SqlConnection conn = new SqlConnection(ReadConfigFile(ActivityConnection)))
            {
                SqlCommand command = new SqlCommand("asp_GetUserNameChangeNotifications", conn);

                try
                {
                    command.CommandType = CommandType.StoredProcedure;
                    conn.Open();

                    // Execute the command and process the results.
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            nameChanges = DataReaderMapToList<NameChanges>(reader);
                        }
                    }

                }
                catch (Exception e)
                {
                    Config.LogError(e);
                }
            }

            return nameChanges;
        }
        public static List<MasteryList> GetMasteryList(int DaysBack)
        {
            var MasteryList = new List<MasteryList>();

            using (SqlConnection conn = new SqlConnection(ReadConfigFile(ActivityConnection)))
            {
                SqlCommand command = new SqlCommand("asp_GetCourseMastery", conn); 
                conn.Open();

                try
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@DaysBack", SqlDbType.Int).Value = DaysBack;

                    // Execute the command and process the results.
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            MasteryList = DataReaderMapToList<MasteryList>(reader);
                        }
                    }
                }
                catch (Exception e)
                {
                    Config.LogError(e);
                }
            }

            return MasteryList;
        }
        public static List<OrganizationGroups> GetOrganizationGroups()
        {
            var OrganizationGroups = new List<OrganizationGroups>();
            using (SqlConnection conn = new SqlConnection(ReadConfigFile(ActivityConnection)))
            {
                SqlCommand command = new SqlCommand("asp_GetEmployeeOrganizationGroups", conn);
                conn.Open();

                try
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Execute the command and process the results.
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            OrganizationGroups = DataReaderMapToList<OrganizationGroups>(reader);
                        }
                    }
                }
                catch
                {
                    throw;
                }
            }

            return OrganizationGroups;
        }
        public static List<CDP> GetCDPForEmployees()
        {
            var CDP = new List<CDP>();
            using (SqlConnection conn = new SqlConnection(ReadConfigFile(ActivityConnection)))
            {
                var command = new SqlCommand();
                
                // Check to see which CDP tables to pull data from
                var IsUseArchived = Convert.ToBoolean(ReadConfigFile(ArchivedCDPTables));
                var CDPYear =  Convert.ToInt16(ReadConfigFile(ArchivedConfigYear));

                if (!(IsUseArchived)){
                    command = new SqlCommand("asp_GetCDPEmployees", conn);
                }
                else{
                    command = new SqlCommand("asp_GetCDPEmployeesArchived", conn);
                    command.Parameters.Add("@CDPYear", SqlDbType.Int).Value = CDPYear;
                }
                    
                conn.Open();

                try
                {
                    command.CommandType = CommandType.StoredProcedure;
                    
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            CDP = DataReaderMapToList<CDP>(reader);
                        }
                    }
                }
                catch
                {
                    throw;
                }
            }

            return CDP;
        }
        public static List<Certification> GetCertificationForEmployees()
        {
            var Certification = new List<Certification>();
            using (SqlConnection conn = new SqlConnection(ReadConfigFile(ActivityConnection)))
            {
                SqlCommand command = new SqlCommand("asp_GetEmployeeCertifications", conn);
                conn.Open();

                try
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Execute the command and process the results.
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            Certification = DataReaderMapToList<Certification>(reader);
                        }
                    }
                }
                catch
                {
                    throw;
                }
            }

            return Certification;
        }
        public static List<Degrees> GetDegreesForEmployees()
        {
            var Degrees = new List<Degrees>();
            using (SqlConnection conn = new SqlConnection(ReadConfigFile(ActivityConnection)))
            {
                SqlCommand command = new SqlCommand("asp_GetEmployeeDegrees", conn);
                conn.Open();
              
                try
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Execute the command and process the results.
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            Degrees = DataReaderMapToList<Degrees>(reader);
                        }
                    }
                }
                catch
                {
                   throw;
                }

                return Degrees;
            }
        }
        public static List<HCMEmployees> GetEmployees()
        {
            var Employees = new List<HCMEmployees>();

            using (SqlConnection conn = new SqlConnection(ReadConfigFile(ActivityConnection)))
            {
                SqlCommand command = new SqlCommand("asp_GetHCMEmployees", conn);
                conn.Open();

                try
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Execute the command and process the results.
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            Employees = DataReaderMapToList<HCMEmployees>(reader);
                        }
                    }
                }
                catch
                {
                    throw;
                }

                return  Employees;
            }           
        }
        public static List<Licenses> GetEmployeesLicenses()
        {
            var PELicenses = new List<Licenses>();

            using (SqlConnection conn = new SqlConnection(ReadConfigFile(ActivityConnection)))
            {
                SqlCommand command = new SqlCommand("asp_GetEmployeeLicenses", conn);
                conn.Open();

                try
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Execute the command and process the results.
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            PELicenses = DataReaderMapToList<Licenses>(reader);
                        }
                    }
                }
                catch
                {
                    throw;

                }
            }

            return PELicenses;
        }
        private static List<T> DataReaderMapToList<T>(IDataReader dr)
        {
            List<T> list = new List<T>();
            try
            {
                T obj = default(T);
                while (dr.Read())
                {
                    obj = Activator.CreateInstance<T>();
                    foreach (PropertyInfo prop in obj.GetType().GetProperties())
                    {
                        if (!object.Equals(dr[prop.Name], DBNull.Value))
                        {
                            prop.SetValue(obj, dr[prop.Name], null);
                        }
                    }
                    list.Add(obj);
                
                }
            }
            catch (Exception e)
            {
                var test = e;
                throw;
            }
            return list;
        }
        public static string ReadConfigFile(string strKey)
        {
            string input = null;

            // Read from confg
            switch (strKey)
            {               
                case CTECConnection:
                    input = ConfigurationManager.ConnectionStrings["CTECConnection"].ToString();
                    break;

                case ActivityConnection:
                    input = ConfigurationManager.ConnectionStrings["ActivityConnection"].ToString();
                    break;

                case DisciplineName:
                    input = ConfigurationManager.AppSettings["ExceptionDisplineArea"].ToString();
                    break;

                case ArchivedCDPTables:
                    input = ConfigurationManager.AppSettings["ArchivedCDPTables"].ToString();
                    break;

                case ArchivedConfigYear:
                    input = ConfigurationManager.AppSettings["ArchivedConfigYear"].ToString();
                    break;
            }
            
            return input;
        }
        public static void SetCourseCompletionProcessedDate(int Id)
        {
            using (SqlConnection conn = new SqlConnection(ReadConfigFile(ActivityConnection)))
            {
                SqlCommand command = new SqlCommand("asp_SetCourseCompletionProcessed", conn);
                conn.Open();

                try
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@Id", SqlDbType.Int).Value = Id;
                    command.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Config.LogError(e);
                }
            }
        }
        public static void LoadCtecDataToDB(int DaysBack)
        {
            var MasteryList = Carollo.GetMasteryList(DaysBack);

            foreach (MasteryList mastery in MasteryList)
            {
                var completion = new CourseCompletion()
                {
                    Vendor = "CTec",
                    UserName_Email = mastery.Email,
                    CourseName = mastery.CourseId,
                    CourseTitle = mastery.CourseTitle,
                    CompletionDate = mastery.CompletedDate,
                    Score = mastery.Score
                };

                InsertCourseDataToDb(completion);
            }
        }
        public static void LoadRedVectorDataToDB(int DaysBack)
        {
            var rvCourses = RedVectorAPI.GetAccountCourseActivity(DaysBack);

            foreach (RVCourseActivity course in rvCourses)
            {
                Double credits = 0;
                var result = Double.TryParse(course.CourseHours, out credits);

                course.CourseItemNumber = course.CourseItemNumber.ToUpper();
                if (!course.CourseItemNumber.StartsWith("RV-"))
                {
                    course.CourseItemNumber = "RV-" + course.CourseItemNumber;
                }

                var completion = new CourseCompletion()
                {
                    Vendor = "Red Vector",
                    UserName_Email = course.UserName,
                    CourseName = course.CourseItemNumber,
                    CourseTitle = course.CourseTitle,
                    CompletionDate = course.CompletionDateUtc,
                    Score = course.UserScorePercent,                    
                    Credits_Hours = Convert.ToByte(credits),
                    TotalTimeSeconds = course.TotalTimeSeconds,
                    EnrollmentDate = course.EnrollmentDateUtc,
                    ExpirationDate = course.ExpirationDateUtc
                };

                InsertCourseDataToDb(completion);
            }
        }
        public static void InsertCourseDataToDb (CourseCompletion Course)
        {
            using (SqlConnection conn = new SqlConnection(ReadConfigFile(ActivityConnection)))
            {
                SqlCommand command = new SqlCommand("asp_InsertCourseData", conn);

                conn.Open();

                try
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.Add("@Vendor", SqlDbType.NVarChar).Value = Course.Vendor;
                    command.Parameters.Add("@UserName_Email", SqlDbType.NVarChar).Value = Course.UserName_Email;
                    command.Parameters.Add("@CourseName", SqlDbType.NVarChar).Value = Course.CourseName;
                    command.Parameters.Add("@CourseTitle", SqlDbType.NVarChar).Value = Course.CourseTitle;
                    command.Parameters.Add("@EnrollmentDate", SqlDbType.DateTime).Value = Course.EnrollmentDate == DateTime.MinValue ? null : Course.EnrollmentDate;
                    command.Parameters.Add("@CompletionDate", SqlDbType.DateTime).Value = Course.CompletionDate;
                    command.Parameters.Add("@Credits_Hours", SqlDbType.TinyInt).Value = Course.Credits_Hours;
                    command.Parameters.Add("@Score", SqlDbType.Decimal).Value = Course.Score;
                    command.Parameters.Add("@TotalTimeSeconds", SqlDbType.Int).Value = Course.TotalTimeSeconds;
                    command.Parameters.Add("@ExpirationDate", SqlDbType.DateTime).Value = Course.ExpirationDate == DateTime.MinValue ? null : Course.ExpirationDate;

                    command.ExecuteNonQuery();
                }
                catch 
                {
                    throw;
                }
            }
        }
        public static void GetEmployeeDataToAbsorb()
        {
            var startDate = DateTime.Now;
            int numberOfEmployeesReviewed = 0;
            int numberOfEmployeesAdd = 0;
            int numberOfEmployeesUpdate = 0;
            int numberOfEmployeesSkip = 0;
            var changes = "";

            try
            {
                var LogActivity = Carollo.GetApplicationActions(ABSORB_LMS);

                // Databases
                var HCMEmployees = new List<HCMEmployees>();
                HCMEmployees = Carollo.GetEmployees();

                var Degrees = new List<Degrees>();
                Degrees = Carollo.GetDegreesForEmployees();

                var Certifications = new List<Certification>();
                Certifications = Carollo.GetCertificationForEmployees();

                var CDP = new List<CDP>();
                CDP = Carollo.GetCDPForEmployees();

                var OrgranizationGroups = new List<OrganizationGroups>();
                OrgranizationGroups = Carollo.GetOrganizationGroups();

                var PELicenses = new List<Licenses>();
                PELicenses = Carollo.GetEmployeesLicenses();

                var OldUserNameChanges = new List<Models.NameChanges>();
                OldUserNameChanges = Carollo.GetUserNameChanges();

                // Absorb
                string token = AbsorbAPI.GetToken();
                if (IsToken(token))
                {
                    var LMSEmployees = new List<LMSEmployees>();
                    LMSEmployees = AbsorbAPI.GetUsers(token);

                    var Departments = new List<Departments>();
                    Departments = AbsorbAPI.GetDepartments(token);

                    // Loop Employees data from DB
                    foreach (HCMEmployees HCMEmployee in HCMEmployees)
                    {
                        changes = "";
                        var action = ProcessActions.Skip;
                        // GET EMPLOYEE NUMBER FROM HCM 
                        var currentLMSEmployee = new LMSEmployees();
                        currentLMSEmployee = LMSEmployees.Find(item => item.EmployeeNumber == HCMEmployee.EmployeeID);

                        if (OldUserNameChanges.Exists(item => item.HCMUserId == HCMEmployee.UserName))
                        {
                            if (currentLMSEmployee.UserName == HCMEmployee.UserName)
                            {
                                Carollo.UpdateUserNameChange(HCMEmployee.UserName);
                            }
                            else
                            {
                                continue;
                            }
                        }

                        var currentEmployeeDegrees = new Degrees();
                        currentEmployeeDegrees = Degrees.Find(item => item.EmployeeID == HCMEmployee.EmployeeID);

                        var currentEmployeeCertifications = new Certification();
                        currentEmployeeCertifications = Certifications.Find(item => item.EmployeeID == HCMEmployee.EmployeeID);

                        var currentEmployeeCDPs = new CDP();
                        currentEmployeeCDPs = CDP.Find(item => item.EmployeeID.ToString() == HCMEmployee.EmployeeID);

                        var currentEmployeeOrganizationGroups = new OrganizationGroups();
                        currentEmployeeOrganizationGroups = OrgranizationGroups.Find(item => item.EmployeeID == HCMEmployee.EmployeeID);

                        var currentEmployeePELicenses = new List<Licenses>();
                        currentEmployeePELicenses = PELicenses.FindAll(item => item.PersonID == HCMEmployee.EmployeeID);

                        if (currentLMSEmployee != null)
                        {
                            if (currentLMSEmployee.ActiveStatus == 1)
                            {
                                continue;
                            }

                            changes += CompareDegrees(ref action, currentEmployeeDegrees, currentLMSEmployee);

                            changes += CompareCertifications(ref action, currentEmployeeCertifications, currentLMSEmployee);

                            changes += CompareCDPs(ref action, currentLMSEmployee, currentEmployeeCDPs);

                            changes += CompareOrganizationGroups(ref action, currentLMSEmployee, currentEmployeeOrganizationGroups);

                            changes += CompareEmployeeInfo(ref action, Departments, currentLMSEmployee, HCMEmployee, LogActivity);

                            changes += ComparePELicenses(ref action, currentEmployeePELicenses, currentLMSEmployee);
                        }
                        else
                        {
                            action = ProcessActions.Add; // COULDN'T FIND EMPLOYEE WITHIN Absorb LMS 
                        }

                        var result = string.Empty;
                        switch (action)
                        {
                            case ProcessActions.Add:
                                var NewEmployee = ConstructAbsorbEmployee(HCMEmployee, Departments, currentEmployeeDegrees, currentEmployeeCertifications, currentEmployeeCDPs, currentEmployeeOrganizationGroups, currentEmployeePELicenses);
                                result = AbsorbAPI.CreateUser(NewEmployee, token);

                                Carollo.WriteToLog("Added employee " + HCMEmployee.NickName + " " + HCMEmployee.LastName + " completed " + " results:" + result, MessageType.Text, Carollo.DetermineAction(LogActivity[0], ActionType.NewEmployee.ToString()), false, HCMEmployee.UserName, "", "", HCMEmployee.EmployeeID, "Absorb", "");
                                numberOfEmployeesAdd++;
                                break;

                            case ProcessActions.Update:
                                var UpdateEmployee = ConstructAbsorbEmployee(currentLMSEmployee, HCMEmployee, Departments, currentEmployeeDegrees, currentEmployeeCertifications, currentEmployeeCDPs, currentEmployeeOrganizationGroups, currentEmployeePELicenses);
                                result = AbsorbAPI.UpdateUser(UpdateEmployee, token);

                                Carollo.WriteToLog("Updated employee " + HCMEmployee.NickName + " " + HCMEmployee.LastName + " completed " + " results:" + result, MessageType.Text, Carollo.DetermineAction(LogActivity[0], ActionType.UpdateEmployee.ToString()), false, HCMEmployee.UserName, "", "", HCMEmployee.EmployeeID, "Absorb", changes);
                                numberOfEmployeesUpdate++;
                                break;

                            case ProcessActions.Skip:
                                numberOfEmployeesSkip++;
                                break;
                        }

                        numberOfEmployeesReviewed++;
                    }

                    Carollo.WriteToLog("Employees Data to Absorb results: Employees Reviewed=" + numberOfEmployeesReviewed + " Employees New=" + numberOfEmployeesAdd + " Employees Upated=" + numberOfEmployeesUpdate + " Employees Skipped=" + numberOfEmployeesSkip, MessageType.Text, ABSORB_LMS, ActionType.ExecuteCompleted.ToString(), true, startDate, "HCM_TO_ABSORB");
                }
                else
                {
                    Carollo.WriteToLog(token, MessageType.Text, ABSORB_LMS, ActionType.GetToken.ToString(), true, startDate, "HCM_TO_ABSORB");
                }

            }
            catch (Exception ex)
            {
                Carollo.WriteToLog(ex.StackTrace, MessageType.Text, ABSORB_LMS, ActionType.WriteToLogError.ToString(), true, startDate, "HCM_TO_ABSORB");
            }


        }
        private static bool IsToken(string token)
        {
            if (token.Contains("=="))
            {
                return true;
            }

            return false;
        }
        private static string CompareDegrees(ref ProcessActions action, Degrees currentEmployeeDegrees, LMSEmployees currentLMSEmployee)
        {
            string LMSDegrees = null;
            string HCMDegrees = null;

            if (currentLMSEmployee.CustomFields.String5 !=null)
                LMSDegrees = currentLMSEmployee.CustomFields.String5.ToString();

            if (currentEmployeeDegrees != null)
                HCMDegrees = currentEmployeeDegrees.Degree;

            if (HCMDegrees != LMSDegrees)
            {
                action = ProcessActions.Update;
                return "Degree: " + HCMDegrees + " vs " + LMSDegrees + Environment.NewLine;
            }

            return "";
        }
        private static string CompareCertifications(ref ProcessActions action, Certification currentEmployeeCertifications, LMSEmployees currentLMSEmployee)
        {
            string LMSCertification2 = null;
            string LMSCertification1 = null;
            string LMSCertification3 = null;

            string HCMCertifications1 = null;
            string HCMCertifications2 = null;
            string HCMCertifications3 = null;

            // if (currentLMSEmployee.CustomFields.String11))
            if (currentLMSEmployee.CustomFields.String11 !=null)
                LMSCertification1 = (currentLMSEmployee.CustomFields.String11.ToString() == string.Empty ? null : currentLMSEmployee.CustomFields.String11.ToString());

            if (currentLMSEmployee.CustomFields.String12 != null)
                LMSCertification2 = (currentLMSEmployee.CustomFields.String12.ToString() == string.Empty ? null : currentLMSEmployee.CustomFields.String12.ToString());

            if (currentLMSEmployee.CustomFields.String13 != null)
                LMSCertification3 = (currentLMSEmployee.CustomFields.String13.ToString() == string.Empty ? null : currentLMSEmployee.CustomFields.String13.ToString());

            if (currentEmployeeCertifications != null)
                ConstructArrayOfCertifications(currentEmployeeCertifications, ref HCMCertifications1, ref HCMCertifications2, ref HCMCertifications3);

            HCMCertifications1 = (HCMCertifications1 == string.Empty ? null : HCMCertifications1);
            HCMCertifications2 = (HCMCertifications2 == string.Empty ? null : HCMCertifications2);
            HCMCertifications3 = (HCMCertifications3 == string.Empty ? null : HCMCertifications3);

            if (HCMCertifications1 != LMSCertification1 || HCMCertifications2 != LMSCertification2 || HCMCertifications3 != LMSCertification3)
                action = ProcessActions.Update;

            string changes = "";
            if (HCMCertifications1 != LMSCertification1)
                changes = "Cert1: " + HCMCertifications1 + " vs " + LMSCertification1 + Environment.NewLine;

            if (HCMCertifications2 != LMSCertification2)
                changes += "Cert2: " + HCMCertifications2 + " vs " + LMSCertification2 + Environment.NewLine;

            if (HCMCertifications3 != LMSCertification3)
                changes += "Cert3: " + HCMCertifications3 + " vs " + LMSCertification3 + Environment.NewLine;

            return changes;
        }
        private static string CompareCDPs(ref ProcessActions action, LMSEmployees currentLMSEmployee, CDP currentEmployeeCDPs)
        {
            string CurrentLevel = "";
            string PrimaryReviewerName = "";

            string LMSCurrentLevel = "";
            string LMSPrimaryReviewerName = "";

            if (currentEmployeeCDPs !=null)
            {
                CurrentLevel = ToStr(currentEmployeeCDPs.CurrentLevel);
                PrimaryReviewerName = ToStr(currentEmployeeCDPs.PrimaryReviewerName);
            }

            LMSCurrentLevel = ToStr(currentLMSEmployee.CustomFields.String15);
            LMSPrimaryReviewerName = ToStr(currentLMSEmployee.CustomFields.String16);

            if (LMSCurrentLevel != CurrentLevel || LMSPrimaryReviewerName != PrimaryReviewerName)
                action = ProcessActions.Update;

            string changes = "";
            if (LMSCurrentLevel != CurrentLevel)
                changes = "Level: " + LMSCurrentLevel + " vs " + CurrentLevel + Environment.NewLine;

            if (LMSPrimaryReviewerName != PrimaryReviewerName)
                changes += "Reviewer: " + LMSPrimaryReviewerName + " vs " + PrimaryReviewerName + Environment.NewLine;

            return changes;
        }
        private static string CompareOrganizationGroups(ref ProcessActions action, LMSEmployees currentLMSEmployee, OrganizationGroups currentEmployeeOrganizationGroups)
        {
            string OrgGroupCode = null;
            string LMSOrgGroupCode = null;

            if (currentEmployeeOrganizationGroups !=null)
                OrgGroupCode = ToStr(currentEmployeeOrganizationGroups.OrgGroupCode);

            if (currentLMSEmployee.CustomFields.String6 != null)
                LMSOrgGroupCode = ToStr(currentLMSEmployee.CustomFields.String6);

            if (LMSOrgGroupCode != OrgGroupCode)
            {
                action = ProcessActions.Update;
                return "OrgGroup: " + LMSOrgGroupCode + " vs " + OrgGroupCode + Environment.NewLine;
            }

            return "";
        }
        private static string CompareEmployeeInfo(ref ProcessActions action, List<Departments> Departments, LMSEmployees currentLMSEmployee, HCMEmployees HCMEmployee, List<LogActivity> LogActivity)
        {
            var officeManger = String.Empty;
            var changes = "";

            if (currentLMSEmployee.EmployeeNumber == HCMEmployee.EmployeeID)
            {
                Guid DepartmentId = DetermineDepartmentId(Departments, HCMEmployee.DisciplineArea, HCMEmployee.FunctionalArea);

                if (currentLMSEmployee.DepartmentId != DepartmentId)
                {
                    action = ProcessActions.Update;
                    changes += "Dept: " + currentLMSEmployee.DepartmentId.ToString() + " vs " + DepartmentId.ToString() + Environment.NewLine;
                }

                if (currentLMSEmployee.FirstName != HCMEmployee.NickName)
                {
                    action = ProcessActions.Update;
                    changes += "FirstName: " + currentLMSEmployee.FirstName + " vs " + HCMEmployee.NickName + Environment.NewLine;
                }

                if (currentLMSEmployee.LastName != HCMEmployee.LastName)
                {
                    action = ProcessActions.Update;
                    changes += "LastName: " + currentLMSEmployee.LastName + " vs " + HCMEmployee.LastName + Environment.NewLine;
                }

                if (currentLMSEmployee.Location != HCMEmployee.DivisionCode)
                {
                    action = ProcessActions.Update;
                    changes += "Location: " + currentLMSEmployee.Location + " vs " + HCMEmployee.DivisionCode + Environment.NewLine;
                }

                if (currentLMSEmployee.JobTitle != HCMEmployee.JobTitle)
                {
                    action = ProcessActions.Update;
                    changes += "JobTitle: " + currentLMSEmployee.JobTitle + " vs " + HCMEmployee.JobTitle + Environment.NewLine;
                }

                if (currentLMSEmployee.DateHired != (String.Format("{0:s}", HCMEmployee.LatestHireDate) == string.Empty ? null : String.Format("{0:s}", HCMEmployee.LatestHireDate)))
                {
                    action = ProcessActions.Update;
                    changes += "DateHired: " + currentLMSEmployee.DateHired + " vs " + String.Format("{0:s}", HCMEmployee.LatestHireDate) + Environment.NewLine;
                }

                if (currentLMSEmployee.DateTerminated != (String.Format("{0:s}", HCMEmployee.LastWorkDate) == string.Empty ? null : String.Format("{0:s}", HCMEmployee.LastWorkDate)))
                {
                    action = ProcessActions.Update;
                    changes += "DateTerm: " + currentLMSEmployee.DateTerminated + " vs " + String.Format("{0:s}", HCMEmployee.LastWorkDate) + Environment.NewLine;
                }

                if (currentLMSEmployee.EmailAddress != HCMEmployee.CompanyEmailAddress)
                {
                    action = ProcessActions.Update;
                    changes += "Email: " + currentLMSEmployee.EmailAddress + " vs " + HCMEmployee.CompanyEmailAddress + Environment.NewLine;
                }

                if (ToStr(currentLMSEmployee.CustomFields.String2) != WorkStatusDescription(HCMEmployee.WorkStatus))
                {
                    action = ProcessActions.Update;
                    changes += "WorkStatus: " + ToStr(currentLMSEmployee.CustomFields.String2) + " vs " + WorkStatusDescription(HCMEmployee.WorkStatus) + Environment.NewLine;
                }

                if (ToStr(currentLMSEmployee.CustomFields.String3) != HCMEmployee.DisciplineArea)
                {
                    action = ProcessActions.Update;
                    changes += "Discipline: " + ToStr(currentLMSEmployee.CustomFields.String3) + " vs " + HCMEmployee.DisciplineArea + Environment.NewLine;
                }

                if (ToStr(currentLMSEmployee.CustomFields.String4) != HCMEmployee.FunctionalArea)
                {
                    action = ProcessActions.Update;
                    changes += "FunctionalArea: " + ToStr(currentLMSEmployee.CustomFields.String4) + " vs " + HCMEmployee.FunctionalArea + Environment.NewLine;
                }

                if (currentLMSEmployee.Gender != Convert.ToInt16(HCMEmployee.GenderCode == "M" ? Gender.Male : Gender.Female))
                {
                    action = ProcessActions.Update;
                    changes += "Gender: " + currentLMSEmployee.Gender.ToString() + " vs " + Convert.ToInt16(HCMEmployee.GenderCode == "M" ? Gender.Male : Gender.Female).ToString() + Environment.NewLine;
                }

                if (currentLMSEmployee.Address != HCMEmployee.Address)
                {
                    action = ProcessActions.Update;
                    changes += "Address: " + currentLMSEmployee.Address + " vs " + HCMEmployee.Address + Environment.NewLine;
                }

                if (currentLMSEmployee.City != HCMEmployee.City)
                {
                    action = ProcessActions.Update;
                    changes += "City: " + currentLMSEmployee.City + " vs " + HCMEmployee.City + Environment.NewLine;
                }

                if (currentLMSEmployee.PostalCode != HCMEmployee.PostalCode)
                {
                    action = ProcessActions.Update;
                    changes += "Zip: " + currentLMSEmployee.PostalCode + " vs " + HCMEmployee.PostalCode + Environment.NewLine;
                }

                if (currentLMSEmployee.CustomFields.String17 != null)
                    officeManger = (currentLMSEmployee.CustomFields.String17.ToString() == string.Empty ? null : currentLMSEmployee.CustomFields.String17.ToString());

                if ((String.IsNullOrEmpty(officeManger) ? string.Empty : officeManger) != (String.IsNullOrEmpty(HCMEmployee.OfficeManager) ? string.Empty : HCMEmployee.OfficeManager))
                {
                    action = ProcessActions.Update;
                    changes += "OfficeManager: " + (String.IsNullOrEmpty(officeManger) ? string.Empty : officeManger) + " vs " + (String.IsNullOrEmpty(HCMEmployee.OfficeManager) ? string.Empty : HCMEmployee.OfficeManager) + Environment.NewLine;
                }

                string LMSTrack = null;
                if (currentLMSEmployee.CustomFields.String14 != null)
                    LMSTrack = currentLMSEmployee.CustomFields.String14.ToString();

                if (LMSTrack != HCMEmployee.Track)
                {
                    action = ProcessActions.Update;
                    changes += "Track: " + LMSTrack + " vs " + HCMEmployee.Track + Environment.NewLine;
                }

                if (currentLMSEmployee.UserName != HCMEmployee.UserName)
                {
                    Carollo.WriteToLog(HCMEmployee.NickName + " " + HCMEmployee.LastName + ": user name changed.", MessageType.Text, Carollo.DetermineAction(LogActivity[0], ActionType.ManualEmployeeUpdate.ToString()), false, HCMEmployee.UserName, string.Empty, string.Empty, string.Empty, "Absorb", currentLMSEmployee.UserName);
                }

            }
            return changes;
        }
        private static string WorkStatusDescription(string workStatus)
        {
            string returnValue = String.Empty;

            if ("24,28,32,36,PRN24,PRN28,PRN32,PRN36,PT2".Contains(workStatus))
                returnValue = "Part-time";

            if ("FT,PT,PRN,2PCTPARTNER".Contains(workStatus))
                returnValue = "Full-time";

            if ("ACAOC,OC,RETPARTNER".Contains(workStatus))
                returnValue = "On-call";

            if ("ACATMP,PTN,TMP".Contains(workStatus))
                returnValue = "Temporary";

            if ("COBR,TER".Contains(workStatus))
                returnValue = "Terminated";

            return returnValue;
        }
        private static Guid DetermineDepartmentId(List<Departments> Departments, string DisciplineArea, string FunctionalArea)
        {
            Departments department = new Departments();
            string arrExceptionOfDiscipline = Carollo.ReadConfigFile(DisciplineName);
            if (arrExceptionOfDiscipline.Contains(DisciplineArea))
            {
                department = Departments.Find(item => item.Name.ToString() == FunctionalArea); // EXCEPTION                
            }
            else
            {
                department = Departments.Find(item => item.Name.ToString() == DisciplineArea);
            }

            return department.Id;
        }
        private static void ConstructArrayOfCertifications(Certification currentEmployeeCertifications, ref string arrCertifications1, ref string arrCertifications2, ref string arrCertifications3)
        {
            int currentCertCount = 1;
            int interation = 1;
            string[] arrayOfCertification = currentEmployeeCertifications.ToString().Split(',');

            if (currentEmployeeCertifications.ArrayOfCertifications.Length >= 255)
            {
                int totalCharCount = 0;
                for (int i = 0; i < arrayOfCertification.Length; i++)
                {
                    string currentCertification = arrayOfCertification[i];
                    int currentCharCount = arrayOfCertification[i].Length;
                    totalCharCount = totalCharCount + currentCharCount;

                    BuildCertifications(ref currentCertCount, ref totalCharCount, ref interation, ref arrCertifications1, ref arrCertifications2, ref arrCertifications3, currentCertification);
                }
            }
            else
            {
                arrCertifications1 = currentEmployeeCertifications.ArrayOfCertifications.ToString();
            }
        }
        private static void BuildCertifications(ref int currentCertCount, ref int totalCharCount, ref int interation, ref string arrCertifications1, ref string arrCertifications2, ref string arrCertifications3, string currentCertification)
        {
            if (totalCharCount <= 255)
            {
                if (currentCertCount == 1)
                {
                    if (interation == 1)
                    {
                        arrCertifications1 = currentCertification;
                    }
                    else
                    {
                        arrCertifications1 = arrCertifications1 + "," + currentCertification;
                    }
                }

                if (currentCertCount == 2)
                {
                    if (interation == 1)
                    {
                        arrCertifications2 = currentCertification;
                    }
                    else
                    {
                        arrCertifications2 = arrCertifications2 + "," + currentCertification;
                    }
                }

                if (currentCertCount == 3)
                {
                    if (interation == 1)
                    {
                        arrCertifications3 = currentCertification;
                    }
                    else
                    {
                        arrCertifications3 = arrCertifications3 + "," + currentCertification;
                    }
                }

                interation++;
            }
            else
            {
                if (currentCertCount == 1)
                    arrCertifications1 = arrCertifications1 + "," + currentCertification;

                if (currentCertCount == 2)
                    arrCertifications2 = arrCertifications2 + "," + currentCertification;

                if (currentCertCount == 3)
                    arrCertifications3 = arrCertifications3 + "," + currentCertification;

                totalCharCount = 0;
                interation = 1;
                currentCertCount++;
            }
        }
        private static string ComparePELicenses(ref ProcessActions action, List<Licenses> currentEmployeePELicenses, LMSEmployees currentLMSEmployee)
        {
            var changes = "";
            string PELicense = null;
            string PEStateRegistered = null;

            string LMSPELicense = null;
            string LMSPEStateRegistered = null;

            if (currentLMSEmployee.CustomFields.String7 != null)
                LMSPELicense = (currentLMSEmployee.CustomFields.String7.ToString() == string.Empty ? null : currentLMSEmployee.CustomFields.String7.ToString());

            if (currentLMSEmployee.CustomFields.String8 != null)
                LMSPEStateRegistered = (currentLMSEmployee.CustomFields.String8.ToString() == string.Empty ? null : currentLMSEmployee.CustomFields.String8.ToString());

            if (currentEmployeePELicenses != null)
            {
                ConstructPELicenses(currentEmployeePELicenses, ref PELicense);
                ConstructPEStateResgistered(currentEmployeePELicenses, ref PEStateRegistered);
            }

            PELicense = (PELicense == string.Empty ? null : PELicense);
            PEStateRegistered = (PEStateRegistered == string.Empty ? null : PEStateRegistered);

            if (PELicense != LMSPELicense || PEStateRegistered != LMSPEStateRegistered)
                action = ProcessActions.Update;

            if (PELicense != LMSPELicense)
                changes = "PELicense: " + PELicense + " vs " + LMSPELicense + Environment.NewLine;

            if (PEStateRegistered != LMSPEStateRegistered)
                changes = "PEStateRegistered: " + PEStateRegistered + " vs " + LMSPEStateRegistered + Environment.NewLine;

            return changes;
        }
        private static void ConstructPELicenses(List<Licenses> currentEmployeePELicenses, ref string PELicense)
        {
            int i = 0;
            PELicense = string.Empty;

            foreach (var License in currentEmployeePELicenses)
            {
                if (!(PELicense.Contains(License.CertificationLevel)))
                {
                    if (i == 0)
                    {
                        PELicense = License.CertificationLevel;
                    }
                    else
                    {
                        PELicense = PELicense + "," + License.CertificationLevel;
                    }

                    i++;
                }
            }
        }
        private static void ConstructPEStateResgistered(List<Licenses> currentEmployeePELicenses, ref string PEStateRegistered)
        {
            int i = 0;
            PEStateRegistered = string.Empty;

            foreach (var License in currentEmployeePELicenses)
            {
                if (!(PEStateRegistered.Contains(License.IssuedByWhoCode)))
                {
                    if (i == 0)
                    {
                        PEStateRegistered = License.IssuedByWhoCode;
                    }
                    else
                    {
                        PEStateRegistered = PEStateRegistered + "," + License.IssuedByWhoCode;
                    }

                    i++;
                }
            }
        }
        private static NewEmployee ConstructAbsorbEmployee(HCMEmployees HCMEmployee, List<Departments> Departments, Degrees currentEmployeeDegrees, Certification currentEmployeeCertifications, CDP currentEmployeeCDPs, OrganizationGroups currentEmployeeOrganizationGroups, List<Licenses> currentEmployeePELicenses)
        {
            string arrCertifications1 = null;
            string arrCertifications2 = null;
            string arrCertifications3 = null;

            string CurrentLevel = null;
            string PrimaryReviewerName = null;
            string Track = null;

            string Degree = null;
            string OrgGroupCode = null;

            string PELicense = null;
            string PEStateRegistered = null;

            if (currentEmployeeCertifications != null)
                ConstructArrayOfCertifications(currentEmployeeCertifications, ref arrCertifications1, ref arrCertifications2, ref arrCertifications3);

            if (currentEmployeeCDPs != null)
            {
                CurrentLevel = currentEmployeeCDPs.CurrentLevel;
                PrimaryReviewerName = currentEmployeeCDPs.PrimaryReviewerName;
                //Track = currentEmployeeCDPs.Track;
            }

            Track = HCMEmployee.Track;

            if (currentEmployeeDegrees != null)
                Degree = currentEmployeeDegrees.Degree;

            if (currentEmployeeOrganizationGroups != null)
                OrgGroupCode = currentEmployeeOrganizationGroups.OrgGroupCode;

            Guid DepartmentId = DetermineDepartmentId(Departments, HCMEmployee.DisciplineArea, HCMEmployee.FunctionalArea);

            // Build PELicenses
            if (currentEmployeePELicenses != null)
            {
                ConstructPELicenses(currentEmployeePELicenses, ref PELicense);
                ConstructPEStateResgistered(currentEmployeePELicenses, ref PEStateRegistered);
            }

            PELicense = (PELicense == string.Empty ? null : PELicense);
            PEStateRegistered = (PEStateRegistered == string.Empty ? null : PEStateRegistered);

            var employee = new NewEmployee()
            {
                UserName = HCMEmployee.UserName,
                Password = BuildNewPassword(HCMEmployee.BirthDate.ToString(), HCMEmployee.LastName),
                FirstName = HCMEmployee.NickName,
                LastName = HCMEmployee.LastName,
                ActiveStatus = Convert.ToInt16(HCMEmployee.ActiveStatus),
                DepartmentId = DepartmentId,
                ExternalId = HCMEmployee.EmployeeID,
                isAdmin = false,
                isInstructor = false,
                isLearner = true,
                LanguageId = 1,
                EmployeeNumber = HCMEmployee.EmployeeID,
                Location = HCMEmployee.DivisionCode,
                JobTitle = HCMEmployee.JobTitle,
                EmailAddress = HCMEmployee.CompanyEmailAddress,
                DateHired = String.Format("{0:s}", HCMEmployee.LatestHireDate),
                DateTerminated = String.Format("{0:s}", HCMEmployee.LastWorkDate),
                Gender = Convert.ToInt16(HCMEmployee.GenderCode == "M" ? Gender.Male : Gender.Female),
                Address = HCMEmployee.Address,
                City = HCMEmployee.City,
                PostalCode = HCMEmployee.PostalCode,
                CustomFields = new CustomFields()
                {
                    DateTime3 = DateTime.Now,
                    String1 = HCMEmployee.Suffix,
                    String2 = WorkStatusDescription(HCMEmployee.WorkStatus),
                    String3 = HCMEmployee.DisciplineArea,
                    String4 = HCMEmployee.FunctionalArea,
                    String5 = Degree,
                    String6 = OrgGroupCode,
                    String7 = PELicense,
                    String8 = PEStateRegistered,
                    String11 = arrCertifications1,
                    String12 = arrCertifications2,
                    String13 = arrCertifications3,
                    String14 = Track,
                    String15 = CurrentLevel,
                    String16 = PrimaryReviewerName,
                    String17 = HCMEmployee.OfficeManager,
                }
            };

            return employee;
        }
        // Use for update only
        private static LMSEmployees ConstructAbsorbEmployee(LMSEmployees LMSEmployee, HCMEmployees HCMEmployee, List<Departments> Departments, Degrees currentEmployeeDegrees, Certification currentEmployeeCertifications, CDP currentEmployeeCDPs, OrganizationGroups currentEmployeeOrganizationGroups, List<Licenses> currentEmployeePELicenses)
        {
            string arrCertifications1 = string.Empty;
            string arrCertifications2 = string.Empty;
            string arrCertifications3 = string.Empty;

            string CurrentLevel = null;
            string PrimaryReviewerName = null;
            string Track = null;

            string Degree = null;
            string OrgGroupCode = null;

            string PELicense = null;
            string PEStateRegistered = null;

            //Get HCM Data

            if (currentEmployeeCertifications != null)
                ConstructArrayOfCertifications(currentEmployeeCertifications, ref arrCertifications1, ref arrCertifications2, ref arrCertifications3);

            if (currentEmployeeCDPs != null)
            {
                CurrentLevel = currentEmployeeCDPs.CurrentLevel;
                PrimaryReviewerName = currentEmployeeCDPs.PrimaryReviewerName;
            }

            Track = HCMEmployee.Track;

            if (currentEmployeeDegrees != null)
                Degree = currentEmployeeDegrees.Degree;

            if (currentEmployeeOrganizationGroups != null)
                OrgGroupCode = currentEmployeeOrganizationGroups.OrgGroupCode;

            Guid DepartmentId = DetermineDepartmentId(Departments, HCMEmployee.DisciplineArea, HCMEmployee.FunctionalArea);

            // Build PELicenses
            if (currentEmployeePELicenses != null)
            {
                ConstructPELicenses(currentEmployeePELicenses, ref PELicense);
                ConstructPEStateResgistered(currentEmployeePELicenses, ref PEStateRegistered);
            }

            PELicense = (PELicense == string.Empty ? null : PELicense);
            PEStateRegistered = (PEStateRegistered == string.Empty ? null : PEStateRegistered);

            //Build Absorb EMployee Record from current HCM data

            var employee = new LMSEmployees()
            {
                Id = LMSEmployee.Id,
                //UserName = HCMEmployee.UserName,
                UserName = LMSEmployee.UserName, // LMS UserName cannot be changed
                FirstName = HCMEmployee.NickName,
                LastName = HCMEmployee.LastName,
                ActiveStatus = Convert.ToInt16(HCMEmployee.ActiveStatus),
                DepartmentId = DepartmentId,
                ExternalId = HCMEmployee.EmployeeID,
                isAdmin = LMSEmployee.isAdmin,
                isInstructor = LMSEmployee.isInstructor,
                isLearner = LMSEmployee.isLearner,
                LanguageId = 1,
                EmployeeNumber = HCMEmployee.EmployeeID,
                Location = HCMEmployee.DivisionCode,
                JobTitle = HCMEmployee.JobTitle,
                EmailAddress = HCMEmployee.CompanyEmailAddress,
                DateHired = String.Format("{0:s}", HCMEmployee.LatestHireDate),
                DateTerminated = String.Format("{0:s}", HCMEmployee.LastWorkDate),
                Gender = Convert.ToInt16(HCMEmployee.GenderCode == "M" ? Gender.Male : Gender.Female),
                Address = HCMEmployee.Address,
                City = HCMEmployee.City,
                PostalCode = HCMEmployee.PostalCode,
                RoleIds = LMSEmployee.RoleIds,
                CustomFields = new CustomFields()
                {
                    DateTime3 = DateTime.Now,
                    String1 = HCMEmployee.Suffix,
                    String2 = WorkStatusDescription(HCMEmployee.WorkStatus),
                    String3 = HCMEmployee.DisciplineArea,
                    String4 = HCMEmployee.FunctionalArea,
                    String5 = Degree,
                    String6 = OrgGroupCode,
                    String7 = PELicense,
                    String8 = PEStateRegistered,
                    String11 = arrCertifications1,
                    String12 = arrCertifications2,
                    String13 = arrCertifications3,
                    String14 = Track,
                    String15 = CurrentLevel,
                    String16 = PrimaryReviewerName,
                    String17 = HCMEmployee.OfficeManager,
                }
            };

            return employee;
        }
        private static string BuildNewPassword(string DOB, string LastName)
        {
            string[] arrBirthDate = DOB.ToString().Split('/');
            string mm = arrBirthDate[0].Length == 1 ? "0" + arrBirthDate[0] : arrBirthDate[0];
            string dd = arrBirthDate[1].Length == 1 ? "0" + arrBirthDate[1] : arrBirthDate[1];
            return LastName + mm + dd;
        }
        private static string ToStr(object Original)
        {
            return Original == null ? "" : Original.ToString();
        }
    }
}

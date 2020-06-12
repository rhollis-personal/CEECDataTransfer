using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.IO;

namespace CeecDataTransfer
{
    public class RedVectorAPI
    {        
        public static List<RVCourseActivity> GetAccountCourseActivity(int DaysBack)
        {
            var result = new List<RVCourseActivity>();
            var arrayOfString = String.Empty;
            var account = new Accounts.RedVectorLMS();
            var endDate = DateTime.Today.ToString("MM/dd/yyyy");
            var startDate = DateTime.Now.Date.AddDays(-DaysBack).ToString("MM/dd/yyyy");

            var UrlDomain = ConfigurationManager.AppSettings["RedVectorUrlDomain"];
            //var Url = UrlDomain + "?WebServiceUsername=" + account.Username + "&WebServicePassword=" + account.Password + "&StartDateUtc=11/01/2016&EndDateUtc=12/1/2016&OnlyCompletions=1";
            var Url = UrlDomain + "?WebServiceUsername=" + account.Username + "&WebServicePassword=" + account.Password + "&StartDateUtc=" + startDate + "&EndDateUtc=" + endDate + "&OnlyCompletions=1";

            var request = WebRequest.Create(Url) as HttpWebRequest;
            request.Method = "GET";        
            request.Timeout = 30000; // 30 seconds

            try
            {
                // Get response  
                var myWebResponse = request.GetResponse() as HttpWebResponse;

                // Get the response stream and Deserialize to Course Activity
                using (StreamReader reader = new StreamReader(myWebResponse.GetResponseStream()))
                {
                    arrayOfString = reader.ReadToEnd();
                    result = DeserializeObject(result, arrayOfString);
                };
               
                return result;
            }
            catch (Exception e)
            {
                Carollo.WriteToLog("Error at RedVectorAPI.GetAccountCourseActivity: " + e.Message + Environment.NewLine + e.InnerException + Environment.NewLine + e.StackTrace, MessageType.Text, "System", ActionType.SetUpLog.ToString(), true, DateTime.Now, "");
                throw;
            }
        }

        private static List<RVCourseActivity> DeserializeObject(List<RVCourseActivity> CourseActivity, string arrayOfString)
        {
            string[] stringSeparators = new string[] { "\r\n" };
            string[] arrays = arrayOfString.Split(stringSeparators, StringSplitOptions.None);

            foreach (var array in arrays)
            {
                if (array.Contains("<string>"))
                {
                    string[] selectedItem = array.Replace("<string>", "").Replace("</string>", "").Split('\t');
                    CourseActivity.Add(CreateFromData(selectedItem));
                }
            }

            return CourseActivity;
        }

        public static RVCourseActivity CreateFromData(string[] data)
        {
            if (data.Length != 14)
            {
                throw new ArgumentException("Unable to convert to RVCourseActivity object!");
            }

            return new RVCourseActivity
            {
                AuthCode = data[0].TrimStart().TrimEnd(),
                UserName = data[1],
                UserNumber = data[2],
                LastName = data[3],
                FirstName = data[4],
                CourseItemNumber = data[5],
                CourseTitle = data[6],
                CourseHours = data[7],
                EnrollmentDateUtc = Convert.ToDateTime(data[8]),
                ExpirationDateUtc = Convert.ToDateTime(data[9]),
                Status = data[10],
                CompletionDateUtc = Convert.ToDateTime(data[11]),
                UserScorePercent = Convert.ToDecimal(data[12]),
                TotalTimeSeconds = Convert.ToInt32(data[13])               
            };
        }
    }
}

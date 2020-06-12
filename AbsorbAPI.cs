using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using System.Web.Script.Serialization;
using CeecDataTransfer.Absorb;
using RestSharp;

namespace CeecDataTransfer
{
    public class AbsorbAPI
    {
        public static string GetToken()
        {
            //This is actually the CEEC.carollo.com account information.
            Accounts.AbsorbLMS account = new Accounts.AbsorbLMS();
            string UrlDomain = ConfigurationManager.AppSettings["AbsorbUrlDomain"];
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var client = new
            RestClient(UrlDomain);
            var request = new RestRequest("Authenticate", Method.POST);
            request.AddHeader("Content-Type", "application/json");

            var credentials = new
            {
                account.Username,
                account.Password,
                account.PrivateKey
            };
            request.AddJsonBody(credentials);
            try
            {
                var response = client.Execute(request);
                var content = response.Content;
                //This removes outer quotes from the token
                var token = content.Trim('"');
                return token;            
            }
            catch (Exception e)
            {
                Carollo.WriteToLog("Error at AbsorbAPI.GetToken: " + e.Message + Environment.NewLine + e.InnerException + Environment.NewLine + e.StackTrace, MessageType.Text, "System", ActionType.WriteToLogError.ToString(), true, DateTime.Now, "");
                throw;
            }
        }

        public bool ExpiredToken()
        {
            DateTime expirationDate;
            if (DateTime.TryParse(Config.ReadSetting("TokenExpirationDate"), out expirationDate) == false)
            {
                expirationDate = DateTime.Now;
            }

            return DateTime.Now >= expirationDate;
        }
        private static HttpWebResponse GetResponse(HttpWebRequest request, bool isToken = false)
        {
            HttpWebResponse response;
            response = null;
            var attemptCount = 1;
            var countLimit = 3;
            var sleepTime = 2;

            if (isToken)
            {
                countLimit = 5;
                sleepTime = 5; //seconds
            }

            while (attemptCount <= countLimit)
            {
                try
                {
                    System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
                    //Carollo.WriteToLog("GetResponse: Security Protocol: " + System.Net.ServicePointManager.SecurityProtocol.ToString(), MessageType.Text, "System", ActionType.SetUpLog.ToString(), true, DateTime.Now, "");
                    response = (HttpWebResponse)request.GetResponse();
                    return response;
                }
                catch (Exception e)
                {
                    Carollo.WriteToLog("Error at AbsorbAPI.GetResponse: Attempt failed - " + attemptCount.ToString(), MessageType.Text, "System", ActionType.SetUpLog.ToString(), true, DateTime.Now, "");
                    if (attemptCount >= countLimit && isToken) // only throw the error if the system is attempting to get the token
                    {
                        Carollo.WriteToLog("Error at AbsorbAPI.GetResponse: " + e.Message + Environment.NewLine + e.InnerException + Environment.NewLine + e.StackTrace, MessageType.Text, "System", ActionType.WriteToLogError.ToString(), true, DateTime.Now, "");
                        throw;
                    }

                    System.Threading.Thread.Sleep(sleepTime * 1000);
                }
                finally
                {
                    attemptCount++;
                }
            }

            return response;
        }
        public static string CreateUser(NewEmployee Employee, string token)
        {
            string jsonString = string.Empty;
            try
            {
                string UrlDomain = ConfigurationManager.AppSettings["AbsorbUrlDomain"];
                string Url = UrlDomain + "createabsorbaccount";
             
                HttpWebRequest request = WebRequest.Create(Url) as HttpWebRequest;
                request.Method = "POST";
                request.ContentType = " text/json";
                request.Headers.Add("Authorization", token);
                request.UserAgent = "Mozilla/4.0 (compatible; MSIE 5.01; Windows NT 5.0)";
                request.Timeout = 30000; // 30 seconds

                var javaScriptSerializer = new JavaScriptSerializer();
                jsonString = javaScriptSerializer.Serialize(Employee);

                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    streamWriter.Write(jsonString);
                    streamWriter.Flush();
                }

                // Get response  
                //HttpWebResponse myWebResponse = (HttpWebResponse)request.GetResponse();                
                HttpWebResponse myWebResponse = GetResponse(request);

                // Get the response stream  
                using (StreamReader reader = new StreamReader(myWebResponse.GetResponseStream()))
                {
                    jsonString = reader.ReadToEnd();
                };
               
            }
            catch (Exception e)
            {
                Carollo.WriteToLog("Error at AbsorbAPI.CreateUser: " + e.Message + Environment.NewLine + e.InnerException + Environment.NewLine + e.StackTrace, MessageType.Text, "System", ActionType.WriteToLogError.ToString(), true, DateTime.Now, "");
                throw;
            }

            return jsonString;
        }
        public static string CreateEnrollment(string token, string UserId, string CourseId)
        {
            try
            {
                string UrlDomain = ConfigurationManager.AppSettings["AbsorbUrlDomain"];
                string Url = UrlDomain + "users/" + UserId + "/enrollments/" + CourseId + "/?reEnroll=true";

                HttpWebRequest request = WebRequest.Create(Url) as HttpWebRequest;
                request.Method = "POST";
                request.ContentType = " text/json";
                request.Headers.Add("Authorization", token);
                request.UserAgent = "Mozilla/4.0 (compatible; MSIE 5.01; Windows NT 5.0)";
                request.Timeout = 30000; // 30 seconds

                string json = "";

                // Get response  
                //var myWebResponse = request.GetResponse() as HttpWebResponse;
                HttpWebResponse myWebResponse = GetResponse(request);
                if (myWebResponse == null)
                {
                    return "";
                }

                var response = new AddEnrollmentResults();
                // Get the response stream and Deserialize to Employee
                using (StreamReader reader = new StreamReader(myWebResponse.GetResponseStream()))
                {
                    json = reader.ReadToEnd();
                    response = JsonConvert.DeserializeObject<AddEnrollmentResults>(json);
                };

                return response.EnrollmentId.ToString();
            }
            catch (Exception e)
            {
                Carollo.WriteToLog("Error at AbsorbAPI.CreateEnrollment: " + e.Message + Environment.NewLine + e.InnerException + Environment.NewLine + e.StackTrace, MessageType.Text, "System", ActionType.WriteToLogError.ToString(), true, DateTime.Now, "");
                throw;
            }
        }
        public static string UpdateUser(LMSEmployees Employee, string token)
        {
            string jsonString = string.Empty;
            
            try
            {
                string UrlDomain = ConfigurationManager.AppSettings["AbsorbUrlDomain"];
                string Url = UrlDomain + "users";                

                HttpWebRequest request = WebRequest.Create(Url) as HttpWebRequest;
                request.Method = "POST";
                request.ContentType = " text/json";
                request.Headers.Add("Authorization", token);
                request.UserAgent = "Mozilla/4.0 (compatible; MSIE 5.01; Windows NT 5.0)";
                request.Timeout = 30000; // 30 seconds

                var javaScriptSerializer = new JavaScriptSerializer();
                jsonString = javaScriptSerializer.Serialize(Employee);
                
                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    streamWriter.Write(jsonString);
                    streamWriter.Flush();
                }
                // Get response  
                //HttpWebResponse myWebResponse = (HttpWebResponse)request.GetResponse();
                HttpWebResponse myWebResponse = GetResponse(request);
                
                // Get the response stream  
                using (StreamReader reader = new StreamReader(myWebResponse.GetResponseStream()))
                {
                    jsonString = reader.ReadToEnd();
                };                
            }
            catch (Exception e)
            {
                Carollo.WriteToLog("Error at AbsorbAPI.UpdateUser: " + e.Message + Environment.NewLine + e.InnerException + Environment.NewLine + e.StackTrace, MessageType.Text, "System", ActionType.WriteToLogError.ToString(), true, DateTime.Now, "");                
                throw;
            }

            return jsonString;
        }
        public static List<Departments> GetDepartments(string token)
        {
            var result = new List<Departments>();
            string UrlDomain = ConfigurationManager.AppSettings["AbsorbUrlDomain"];
            string Url = UrlDomain + "departments";

            HttpWebRequest request = WebRequest.Create(Url) as HttpWebRequest;
            request.Method = "GET";
            request.ContentType = "application/json";
            request.Headers.Add("Authorization", token);
            request.Timeout = 30000; // 30 seconds

            try
            {
                // Get response  
                //var myWebResponse = request.GetResponse() as HttpWebResponse;
                HttpWebResponse myWebResponse = GetResponse(request);

                // Get the response stream and Deserialize to Employee
                using (StreamReader reader = new StreamReader(myWebResponse.GetResponseStream()))
                {
                    string json = reader.ReadToEnd();
                    result = JsonConvert.DeserializeObject<List<Departments>>(json);
                };

                return result;
            }
            catch (Exception e)
            {
                Carollo.WriteToLog("Error at AbsorbAPI.GetDepartments: " + e.Message + Environment.NewLine + e.InnerException + Environment.NewLine + e.StackTrace, MessageType.Text, "System", ActionType.WriteToLogError.ToString(), true, DateTime.Now, "");
                throw;
            }
        }
        public static List<CustomFieldMapping> GetCustomFieldMapping(string token)
        {
            var result = new List<CustomFieldMapping>();
            string UrlDomain = ConfigurationManager.AppSettings["AbsorbUrlDomain"];
            string Url = UrlDomain + "CustomFieldDefinitions";

            HttpWebRequest request = WebRequest.Create(Url) as HttpWebRequest;
            request.Method = "GET";
            request.ContentType = "application/json";
            request.Headers.Add("Authorization", token);

            try
            {
                // Get response  
                //var myWebResponse = request.GetResponse() as HttpWebResponse;
                HttpWebResponse myWebResponse = GetResponse(request);

                // Get the response stream and Deserialize to Employee
                using (StreamReader reader = new StreamReader(myWebResponse.GetResponseStream()))
                {
                    string json = reader.ReadToEnd();
                    result = JsonConvert.DeserializeObject<List<CustomFieldMapping>>(json);
                };

                return result;
            }
            catch (Exception e)
            {
                Carollo.WriteToLog("Error at AbsorbAPI.GetCustomFieldMapping: " + e.Message + Environment.NewLine + e.InnerException + Environment.NewLine + e.StackTrace, MessageType.Text, "System", ActionType.WriteToLogError.ToString(), true, DateTime.Now, "");
                throw;
            }
        }
        public static List<LMSEmployees> GetUsers(string token)
        {
            var result = new List<LMSEmployees>();
            string UrlDomain = ConfigurationManager.AppSettings["AbsorbUrlDomain"];
            string Url = UrlDomain + "Users";

            HttpWebRequest request = WebRequest.Create(Url) as HttpWebRequest;
            request.Method = "GET";
            request.ContentType = "application/json";
            request.Headers.Add("Authorization", token);
            request.Timeout = 60000; // 30 seconds

            try
            {                
                string json = "";

                // Get response  
                //var myWebResponse = request.GetResponse() as HttpWebResponse;
                HttpWebResponse myWebResponse = GetResponse(request);

                // Get the response stream and Deserialize to Employee
                using (StreamReader reader = new StreamReader(myWebResponse.GetResponseStream()))
                {
                    json = reader.ReadToEnd();
                    result = JsonConvert.DeserializeObject<List<LMSEmployees>>(json);
                };

                return result;
            }
            catch (Exception e)
            {
                Carollo.WriteToLog("Error at AbsorbAPI.GetUsers: " + e.Message + Environment.NewLine + e.InnerException + Environment.NewLine + e.StackTrace, MessageType.Text, "System", ActionType.WriteToLogError.ToString(), true, DateTime.Now, "");
                throw;
            }
        }
        public static List<Curriculum> GetCurriculums(string token)
        {
            var result = new List<Curriculum>();
            string UrlDomain = ConfigurationManager.AppSettings["AbsorbUrlDomain"];
            string Url = UrlDomain + "Curriculums";

            HttpWebRequest request = WebRequest.Create(Url) as HttpWebRequest;
            request.Method = "GET";
            request.ContentType = "application/json";
            request.Headers.Add("Authorization", token);
            request.Timeout = 30000; // 30 seconds

            try
            {
                string json = "";

                // Get response  
                //var myWebResponse = request.GetResponse() as HttpWebResponse;
                HttpWebResponse myWebResponse = GetResponse(request);

                // Get the response stream and Deserialize to Employee
                using (StreamReader reader = new StreamReader(myWebResponse.GetResponseStream()))
                {
                    json = reader.ReadToEnd();
                    result = JsonConvert.DeserializeObject<List<Curriculum>>(json);
                };

                return result;
            }
            catch (Exception e)
            {
                Carollo.WriteToLog("Error at AbsorbAPI.GetCurriculums: " + e.Message + Environment.NewLine + e.InnerException + Environment.NewLine + e.StackTrace, MessageType.Text, "System", ActionType.WriteToLogError.ToString(), true, DateTime.Now, "");
                throw;
            }
        }
        public static List<Courses> GetCourses(string token)
        {
            var result = new List<Courses>();
            string UrlDomain = ConfigurationManager.AppSettings["AbsorbUrlDomain"];
            string Url = UrlDomain + "Courses";

            HttpWebRequest request = WebRequest.Create(Url) as HttpWebRequest;
            request.Method = "GET";
            request.ContentType = "application/json";
            request.Headers.Add("Authorization", token);
            request.Timeout = 30000; // 30 seconds

            try
            {
                string json = "";

                // Get response                   
                //var myWebResponse = request.GetResponse() as HttpWebResponse;
                HttpWebResponse myWebResponse = GetResponse(request);

                // Get the response stream and Deserialize to Employee
                using (StreamReader reader = new StreamReader(myWebResponse.GetResponseStream()))
                {
                    json = reader.ReadToEnd();

                    result = JsonConvert.DeserializeObject<List<Courses>>(json);
                };

                foreach (var course in result)
                {
                    if (course.ExternalId == null)
                    {
                        course.ExternalId = string.Empty;
                    }
                }

                return result;
            }
            catch (Exception e)
            {
                Carollo.WriteToLog("Error at AbsorbAPI.GetCourses: " + e.Message + Environment.NewLine + e.InnerException + Environment.NewLine + e.StackTrace, MessageType.Text, "System", ActionType.WriteToLogError.ToString(), true, DateTime.Now, "");
                throw;
            }
        }
        public static List<Lessons> GetLessons(string token)
        {
            var result = new List<Lessons>();
            string UrlDomain = ConfigurationManager.AppSettings["AbsorbUrlDomain"];
            string Url = UrlDomain + "Lessons";

            HttpWebRequest request = WebRequest.Create(Url) as HttpWebRequest;
            request.Method = "GET";
            request.ContentType = "application/json";
            request.Headers.Add("Authorization", token);

            try
            {
                string json = "";

                // Get response                  
                //var myWebResponse = request.GetResponse() as HttpWebResponse;
                HttpWebResponse myWebResponse = GetResponse(request);

                // Get the response stream and Deserialize to Employee
                using (StreamReader reader = new StreamReader(myWebResponse.GetResponseStream()))
                {
                    json = reader.ReadToEnd();
                    result = JsonConvert.DeserializeObject<List<Lessons>>(json);
                };

                return result;
            }
            catch (Exception e)
            {
                Carollo.WriteToLog("Error at AbsorbAPI.GetLessons: " + e.Message + Environment.NewLine + e.InnerException + Environment.NewLine + e.StackTrace, MessageType.Text, "System", ActionType.WriteToLogError.ToString(), true, DateTime.Now, "");
                throw;
            }
        }
        public static List<Enrollments> GetUserEnrollments(string token,Guid UserId)
        {
            var result = new List<Enrollments>();
            string UrlDomain = ConfigurationManager.AppSettings["AbsorbUrlDomain"];
            string Url = UrlDomain + "users/" + UserId  + "/enrollments";

            HttpWebRequest request = WebRequest.Create(Url) as HttpWebRequest;
            request.Method = "GET";
            request.ContentType = "application/json";
            request.Headers.Add("Authorization", token);
            request.Timeout = 30000; // 30 seconds

            try
            {
                string json = "";

                // Get response  
                //var myWebResponse = request.GetResponse() as HttpWebResponse;
                HttpWebResponse myWebResponse = GetResponse(request);

                // Get the response stream and Deserialize to Employee
                using (StreamReader reader = new StreamReader(myWebResponse.GetResponseStream()))
                {
                    json = reader.ReadToEnd();
                    result = JsonConvert.DeserializeObject<List<Enrollments>>(json);
                };

                return result;
            }
            catch (Exception e)
            {
                Carollo.WriteToLog("Error at AbsorbAPI.GetUserEnrollments: " + e.Message + Environment.NewLine + e.InnerException + Environment.NewLine + e.StackTrace, MessageType.Text, "System", ActionType.WriteToLogError.ToString(), true, DateTime.Now, "");
                throw;
            }
        }
        public static string UpdateEnrollmentCourse(string token, Guid UserId, CourseEnrollment CourseEnrollmentEmployee)
        {
            string jsonString = string.Empty;
            try
            {
                string UrlDomain = ConfigurationManager.AppSettings["AbsorbUrlDomain"];
                string Url = UrlDomain + "users/" + UserId + "/enrollments";

                HttpWebRequest request = WebRequest.Create(Url) as HttpWebRequest;
                request.Method = "POST";
                request.ContentType = " text/json";
                request.Headers.Add("Authorization", token);
                request.UserAgent = "Mozilla/4.0 (compatible; MSIE 5.01; Windows NT 5.0)";
                request.Timeout = 30000; // 30 seconds

                var javaScriptSerializer = new JavaScriptSerializer();
                jsonString = javaScriptSerializer.Serialize(CourseEnrollmentEmployee);

                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    streamWriter.Write(jsonString);
                    streamWriter.Flush();
                }

                // Get response  
                //HttpWebResponse myWebResponse = (HttpWebResponse)request.GetResponse();
                HttpWebResponse myWebResponse = GetResponse(request);

                // Get the response stream  
                using (StreamReader reader = new StreamReader(myWebResponse.GetResponseStream()))
                {
                    jsonString = reader.ReadToEnd();
                };

            }
            catch (Exception e)
            {
                Carollo.WriteToLog("Error at AbsorbAPI.UpdateEnrollmentCourse: " + e.Message + Environment.NewLine + e.InnerException + Environment.NewLine + e.StackTrace, MessageType.Text, "System", ActionType.WriteToLogError.ToString(), true, DateTime.Now, "");
                throw;
            }

            return jsonString;
        }     
        
    }
}

using System;
using System.Collections.Generic;

namespace CeecDataTransfer
{
    public class CourseProcessing
    {
        public static void Process(string Vendor)
        {
            var startDate = DateTime.Now;
            var processDate = DateTime.Now;
            int numberOfCoursesReviewed = 0;
            int numberOfCourseShell = 0;
            int numberOfEnrollments = 0;
            var vendorName = Vendor.Replace(" LMS", "");

            try
            {
                var absorbCourses = new List<Courses>();
                var LogActivity = Carollo.GetApplicationActions(Vendor);

                if (Carollo.IsLogSetUpForApp(LogActivity))
                {
                    // Absorb
                    var LMSEmployees = new List<LMSEmployees>();
                    string token = AbsorbAPI.GetToken();
                    if (IsToken(token))
                    {
                        LMSEmployees = AbsorbAPI.GetUsers(token);
                    }
                    else
                    {
                        Carollo.WriteToLog("CEEC token not valid.", MessageType.Text, Vendor, ActionType.SetUpLog.ToString(), true, processDate, vendorName);
                        return;
                    }

                    var Completions = Carollo.GetCourseCompletions(vendorName);
                    if (Completions.Count == 0)
                    {
                        Carollo.WriteToLog("No completion data.", MessageType.Text, Vendor, ActionType.SetUpLog.ToString(), true, processDate, vendorName);
                        return;
                    }

                    // Group users together for user lookups
                    Completions.Sort((x, y) => x.UserName_Email.CompareTo(y.UserName_Email));
                    var currentUserName = "";
                    var currentUserHasHCMData = false;
                    var currentUserId = "";

                    var HCMEmployees = new List<HCMEmployees>();
                    HCMEmployees = Carollo.GetEmployees();

                    var userEnrollments = new List<Enrollments>();
                    var AbsorbUser = new LMSEmployees();
                    var currentHCMEmployee = new HCMEmployees();

                    foreach (var completion in Completions)
                    {
                        numberOfCoursesReviewed++;

                        if (completion.UserName_Email != currentUserName)
                        {
                            currentUserHasHCMData = false;
                            currentHCMEmployee = HCMEmployees.Find(item => item.UserName.ToLower() == completion.UserName_Email.ToLower());

                            if (currentHCMEmployee != null)
                            {
                                AbsorbUser = LMSEmployees.Find(item => item.UserName.ToLower() == completion.UserName_Email.ToLower());
                                if (AbsorbUser == null)
                                {
                                    continue;
                                }

                                userEnrollments = AbsorbAPI.GetUserEnrollments(token, AbsorbUser.Id.Value);
                                currentUserHasHCMData = true;
                                currentUserId = AbsorbUser.Id.ToString();
                            }
                            else
                            {
                                Carollo.SetCourseCompletionProcessedDate((int)completion.Id);
                                Carollo.WriteToLog("No employee exist in HCM that matches the completion data for user " + completion.UserName_Email + ".", MessageType.Text, Vendor, ActionType.SetUpLog.ToString(), true, processDate, vendorName);
                                continue;
                            }

                            currentUserName = completion.UserName_Email;
                        }

                        if (currentUserHasHCMData)
                        {
                            // fetch data only if needed and only once
                            if (absorbCourses.Count == 0)
                            {
                                absorbCourses = AbsorbAPI.GetCourses(token);
                            }

                            var activityCourse = new Courses();
                            if (absorbCourses.Exists(item => item.ExternalId.ToUpper() == completion.CourseName.ToUpper()))
                            {
                                activityCourse = absorbCourses.Find(item => item.ExternalId.ToUpper() == completion.CourseName.ToUpper());
                            }

                            if (activityCourse.ExternalId != null)
                            {
                                var needsUpdate = false;
                                var userEnrollment = new Enrollments();
                                if (!userEnrollments.Exists(item => item.CourseId == activityCourse.Id && item.DateCompleted == completion.CompletionDate))
                                {
                                    var enrollment = AbsorbAPI.CreateEnrollment(token, currentUserId, activityCourse.Id.ToString());
                                    if (enrollment == "")
                                    {
                                        continue;
                                    }

                                    Guid enrollmentGuid;
                                    if (Guid.TryParse(enrollment, out enrollmentGuid))
                                    {
                                        numberOfEnrollments++;
                                        needsUpdate = true;
                                        userEnrollment = new Enrollments { Id = enrollmentGuid, CourseId = activityCourse.Id, UserId = (Guid)AbsorbUser.Id, IsActive = true, Progress = 1, Score = completion.Score, DateStarted = DateTime.Now, DateCompleted = completion.CompletionDate, TimeSpentTicks = 0, AcceptedTermsAndConditions = false };
                                        Carollo.WriteToLog("Enrollment succeeded: " + enrollment, MessageType.Text, Carollo.DetermineAction(LogActivity[0], ActionType.UpdateEnrollment.ToString()), true, currentUserId, completion.CourseName, completion.CourseTitle, completion.UserName_Email, vendorName, "");
                                    }
                                    else
                                    {
                                        Carollo.WriteToLog("Enrollment failed.", MessageType.Text, Carollo.DetermineAction(LogActivity[0], ActionType.WriteToLogError.ToString()), true, currentUserId, completion.CourseName, completion.CourseTitle, completion.UserName_Email, vendorName, "");
                                    }
                                }
                                else
                                {
                                    userEnrollment = userEnrollments.Find(item => item.CourseId == activityCourse.Id && item.DateCompleted == completion.CompletionDate);
                                    if (userEnrollment.Status != Status.Complete)
                                    {
                                        needsUpdate = true;
                                    }
                                }

                                if (needsUpdate)
                                {
                                    var CourseEnrollment = ConstructCourseEnrollmentEmployee(userEnrollment);
                                    var UpdateResult = AbsorbAPI.UpdateEnrollmentCourse(token, (Guid)AbsorbUser.Id, CourseEnrollment);
                                    Carollo.WriteToLog(currentHCMEmployee.NickName + " " + currentHCMEmployee.LastName + " completed " + userEnrollment.CourseName + " results:" + UpdateResult, MessageType.Text, Carollo.DetermineAction(LogActivity[0], ActionType.UpdateEnrollment.ToString()), false, currentHCMEmployee.UserName, userEnrollment.CourseId.ToString(), userEnrollment.CourseName, currentHCMEmployee.EmployeeID, vendorName, "");
                                }

                                Carollo.SetCourseCompletionProcessedDate((int)completion.Id);
                            }
                            else
                            {
                                numberOfCourseShell++;
                                Carollo.WriteToLog("No course shell exist in CEEC that matches the completion data", MessageType.Text, Carollo.DetermineAction(LogActivity[0], ActionType.CourseShell.ToString()), true, currentUserId, completion.CourseName, completion.CourseTitle, completion.UserName_Email, vendorName, "");
                            }
                        }
                        else
                        {
                            Carollo.WriteToLog("No employee exist in HCM that matches the completion data for user " + completion.UserName_Email + ".", MessageType.Text, Vendor, ActionType.SetUpLog.ToString(), true, processDate, vendorName);
                        }
                    }

                    Carollo.WriteToLog("External completion Courses to CEEC results: Courses Reviewed = " + numberOfCoursesReviewed + " - Courses Enrolled = " + numberOfEnrollments + " - Course Shells missing = " + numberOfCourseShell, MessageType.Text, Vendor, ActionType.ExecuteCompleted.ToString(), true, startDate, vendorName);
                }
                else
                {
                    Carollo.WriteToLog("The Log Activity is not setup for this application", MessageType.Text, Vendor, ActionType.SetUpLog.ToString(), true, startDate, vendorName);
                }
            }
            catch (Exception ex)
            {
                Carollo.WriteToLog(ex.StackTrace, MessageType.Text, Vendor, ActionType.WriteToLogError.ToString(), true, startDate, vendorName);
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

        private static CourseEnrollment ConstructCourseEnrollmentEmployee(Enrollments selectedEnrollments)
        {
            try
            {
                var Course = new CourseEnrollment()
                {
                    Id = selectedEnrollments.Id,
                    CourseId = selectedEnrollments.CourseId,
                    Progress = selectedEnrollments.Progress,
                    Score = selectedEnrollments.Score,
                    Status = Status.Complete,
                    DateCompleted = String.Format("{0:s}", selectedEnrollments.DateCompleted),
                    DateExpires = String.Format("{0:s}", selectedEnrollments.DateExpires),
                    TimeSpent = selectedEnrollments.TimeSpent,
                    DateStarted = selectedEnrollments.DateStarted,
                    Credits = selectedEnrollments.Credits
                };

                return Course;
            }
            catch 
            {
                throw;
            }
        }

        private static CourseEnrollment ConstructCourseEnrollmentEmployee(Guid enrollmentId, Guid courseId, decimal? progress, decimal? score, DateTime? dateCompleted, DateTime? dateExpires, TimeSpan timeSpent, DateTime? dateStarted, decimal? credits)
        {
            try
            {
                var Course = new CourseEnrollment()
                {
                    Id = enrollmentId,
                    CourseId = courseId,
                    Progress = progress,
                    Score = score,
                    Status = Status.Complete,
                    DateCompleted = String.Format("{0:s}", dateCompleted),
                    DateExpires = String.Format("{0:s}", dateExpires),
                    TimeSpent = timeSpent,
                    DateStarted = dateStarted,
                    Credits = credits
                };

                return Course;
            }
            catch 
            {
                throw;
            }
        }
    }
}

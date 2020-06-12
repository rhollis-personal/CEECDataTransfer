using System;

namespace CeecDataTransfer
{
    public class RVCourseActivity
    {
        public string AuthCode { get; set; }
        public string UserName { get; set; }
        public string UserNumber { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public  string CourseItemNumber { get; set; }
        public string CourseTitle { get; set; }
        public string CourseHours { get; set; }
        public Guid CourseId { get; set; }
        public DateTime EnrollmentDateUtc { get; set; }
        public DateTime ExpirationDateUtc { get; set; }
        public string Status { get; set; }
        public DateTime CompletionDateUtc { get; set; }
        public decimal UserScorePercent { get; set; }
        public int TotalTimeSeconds { get; set; }
        public Guid UserId { get; set; }
    }
}

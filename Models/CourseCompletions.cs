using System;

namespace CeecDataTransfer.Models
{
    public class CourseCompletion
    {
        public int? Id { get; set; }
        public string Vendor { get; set; }
        public string UserName_Email { get; set; }
        public string CourseName { get; set; }
        public string CourseTitle { get; set; }
        public DateTime? EnrollmentDate { get; set; }
        public DateTime CompletionDate { get; set; }
        public byte? Credits_Hours { get; set; }
        public decimal? Score { get; set; }
        public int? TotalTimeSeconds { get; set; }
        public DateTime? ExpirationDate { get; set; }
    }
}

using System;

namespace CeecDataTransfer
{
    public class Enrollments
    {
        public Guid Id { get; set; }
        public Guid CourseId { get; set; }
        public string CourseName { get; set; }
        public decimal? Progress { get; set; }
        public decimal? Score { get; set; }
        public Status Status { get; set; }
        public DateTime? DateCompleted { get; set; }
        public DateTime? DateExpires { get; set; }
        public string FullName { get; set; }
        public string JobTitle { get; set; }
        public Guid? CourseVersionId { get; set; }
        public Guid UserId { get; set; }
        public bool? AcceptedTermsAndConditions { get; set; }
        public Int64? TimeSpentTicks { get; set; }
        public TimeSpan TimeSpent { get; set; }
        public DateTime? DateStarted { get; set; }
        public Guid? EnrollmentKeyId { get; set; }
        public Guid? CertificateId { get; set; }
        public decimal? Credits { get; set; }
        public bool? IsActive { get; set; }
    }
}

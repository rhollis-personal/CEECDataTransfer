using System;
using System.Collections.Generic;

namespace CeecDataTransfer
{
    public class Courses 
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Notes { get; set; }
        public string ExternalId { get; set; }
        public DateTime? AccessDate { get; set; }
        public ExpireType ExpireType { get; set; }
        public ExpireDuration ExpireDuration { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public ActiveStatus ActiveStatus { get; set; }
        public List<Guid?> TagIds { get; set; }
        public List<Guid?> ResourceIds { get; set; }
        public List<Guid?> EditorIds { get; set; }
        public List<Prices> Prices { get; set; }
        public List<Guid?> CompetencyDefinitionIds { get; set; }
        public List<Guid?> PrerequisiteCourseIds { get; set; }
        public List<Guid?> PostEnrollmentCourseIds { get; set; }
        public bool AllowCourseEvaluation { get; set; }
        public Guid? CategoryId { get; set; }
        public string CertificateUrl { get; set; }
        public string Audience { get; set; }
        public string Goals { get; set; }
        public string Vendor { get; set; }
        public decimal? CompanyCost { get; set; }
        public decimal? LearnerCost { get; set; }
        public decimal? CompanyTime { get; set; }
        public decimal? LearnerTime { get; set; }
    }

    public class CourseEnrollment
    {
        public Guid Id { get; set; }
        public Guid CourseId { get; set; }
        //public string CourseName { get; set; }
        public decimal? Progress { get; set; }
        public decimal? Score { get; set; }
        public Status Status { get; set; }
        public string DateCompleted { get; set; }
        public string DateExpires { get; set; }
        //public string FullName { get; set; }
        //public string JobTitle { get; set; }
        //public Guid? CourseVersionId { get; set; }
        //public Guid UserId { get; set; }
        //public bool AcceptedTermsAndConditions { get; set; }
        //public Int64 TimeSpentTicks { get; set; }
        public TimeSpan TimeSpent { get; set; }
        public DateTime? DateStarted { get; set; }
        //public Guid? EnrollmentKeyId { get; set; }
        //public Guid? CertificateId { get; set; }
        public decimal? Credits { get; set; }
        //public bool IsActive { get; set; }
    }
}

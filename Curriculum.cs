using System;
using System.Collections.Generic;

namespace CeecDataTransfer
{
    public class Curriculum
    {
        public bool? IsPacingEnabled { get; set; }
        public List<Guid?> CurriculumGroupIds { get; set; }
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

    public class Prices
    {
        public Guid? Id { get; set; }
        public Guid? DepartmentId { get; set; }
        public string Currency { get; set; }
        public decimal? Amount { get; set; }
    }
    public enum ActiveStatus
    {
        Active = 0,
        Inactive = 1
    }

    public class ExpireDuration
    {
        public int Years { get; set; }
        public int Months { get; set; }
        public int Days { get; set; }
    }
    public enum ExpireType
    {
        None = 0,
        Date = 1,
        Duration = 2
    }

    public enum Status
    {
        NotStarted = 0,
        InProgress = 1,
        PendingApproval = 2,
        Complete = 3,
        NotComplete = 4,
        Failed = 5,
        Declined = 6,
        PendingEvaluationRequired = 7,
        OnWaitlist = 8,
        Absent = 9,
        NotApplicabl = 10,
        PendingProctor = 11
    }
}

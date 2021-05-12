using System;
using System.Collections.Generic;

namespace CeecDataTransfer
{
    public class NewEmployee
    {
        public Guid DepartmentId { get; set; }
        public string FirstName { get; set; }
        public string Middle { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string EmailAddress { get; set; }
        public int? LanguageId { get; set; }
        public int Gender { get; set; }
        public string Address { get; set; }
        public string Addrss2 { get; set; }
        public string City { get; set; }
        public int? ProvinceId { get; set; }
        public int? CountryId { get; set; }
        public string PostalCode { get; set; }
        public string Phone { get; set; }
        public string EmployeeNumber { get; set; }
        public string Location { get; set; }
        public string JobTitle { get; set; }
        public string ReferenceNumber { get; set; }
        public string DateHired { get; set; }
        public string DateTerminated { get; set; }
        public string Notes { get; set; }
        public CustomFields CustomFields { get; set; }
        public int ActiveStatus { get; set; }
        public bool isLearner { get; set; }
        public bool isAdmin { get; set; }
        public bool isInstructor { get; set; }
        public string ExternalId { get; set; }
        public Guid? SupervisorId { get; set; }       
    }

    public class LMSEmployees
    {
        public Guid? Id { get; set; }
        public Guid DepartmentId { get; set; }
        public string FirstName { get; set; }
        public string Middle { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string EmailAddress { get; set; }
        public int? LanguageId { get; set; }
        public int Gender { get; set; }
        public string Address { get; set; }
        public string Addrss2 { get; set; }
        public string City { get; set; }
        public int? ProvinceId { get; set; }
        public int? CountryId { get; set; }
        public string PostalCode { get; set; }
        public string Phone { get; set; }
        public string EmployeeNumber { get; set; }
        public string Location { get; set; }
        public string JobTitle { get; set; }
        public string ReferenceNumber { get; set; }
        public string DateHired { get; set; }
        public string DateTerminated { get; set; }
        public string Notes { get; set; }
        public CustomFields CustomFields { get; set; }
        public List<string> RoleIds { get; set; }
        public int ActiveStatus { get; set; }
        public bool isLearner { get; set; }
        public bool isAdmin { get; set; }
        public bool isInstructor { get; set; }
        public string ExternalId { get; set; }
        public Guid? SupervisorId { get; set; }      
    }
   
    public class HCMEmployees
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string NickName { get; set; }
        public string LastName { get; set; }        
        public string ActiveStatus { get; set; }
        public string DisciplineArea { get; set; }
        public string FunctionalArea { get; set; }
        public string EmployeeID { get; set; }
        public string CompanyEmailAddress { get; set; }
        public string Suffix { get; set; }
        public string DivisionCode { get; set; }
        public string JobTitle { get; set; }
        public bool PersonStatusCurrentFlag { get; set; } 
        public string WorkStatus { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string StateProvinceCode { get; set; }
        public string PostalCode { get; set; }              
        public DateTime LatestHireDate { get; set; }      
        public DateTime? LastWorkDate { get; set; }
        public string GenderCode { get; set; }
        public string OfficeManager { get; set; }
        public DateTime BirthDate { get; set; }
        public string Track { get; set; }
    }
     
    public class OrganizationGroups
    {
        public string EmployeeID { get; set; }
        public string OrgGroupCode { get; set;}
        public string OrgLevelCode { get; set; }
    }

    public class Licenses
    {
        public string PersonID { get; set; }
        public string CertificationLevel { get; set; }
        public string CertificationComments { get; set; }
        public string CertificationCode { get; set; }
        public string IssuedByWhoCode { get; set; }
    }

    public class CDP
    {
        public int EmployeeID { get; set; }
        //public string Track { get; set; }
        public string PrimaryReviewerName { get; set; }
        public string CurrentLevel { get; set; }
    }

    public class Certifications
    {
        public string EmployeeID { get; set; }
        public string Certification1 { get; set; }
        public string Certification2 { get; set; }
        public string Certification3 { get; set; }
    }
    
    public class Certification
    {
        public string EmployeeID { get; set; }
        public string ArrayOfCertifications { get; set; }
    }

    public class Degrees
    {
        public string EmployeeID { get; set; }
        public string Degree { get; set; }
    }

    public enum Gender
    {
        Male = 1,
        Female = 2
    }

    public class MasteryList
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CourseId { get; set; }
        public string CourseTitle { get; set; }
        //public string ActionType { get; set; }
        public DateTime CompletedDate { get; set; }
        public string Email { get; set; }
        public decimal Score { get; set; }
    }
}

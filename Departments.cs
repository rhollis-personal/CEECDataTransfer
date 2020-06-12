using System;

namespace CeecDataTransfer
{
    public class Departments
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool UseDepartmentContactDetails { get; set; }
        public string CompanyName { get; set; }
        public string PhoneNumber { get; set; }
        public string EmailAddress { get; set; }
        public int? ExternalId { get; set; }
        public Guid? ParentId { get; set; }
        public Guid? CurrencyId { get; set; }
    }
}

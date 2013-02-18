using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ValidationAndConcurrencyDemo.Models
{
    public class Employee : IValidatableObject
    {
        protected const string EMAIL_SUFFIX = "@example.com";

        public int EmployeeId { get; set; }
        public string Name { get; set; }
        [Required]
        public string Username { get; set; }
        public int Grade { get; set; }
        public string PhoneNumber { get; set; }
        public Department Department { get; set; }
        public int DepartmentId { get; set; }
        [Timestamp]
        public byte[] RowVersion { get; set; }
        
        public string Email
        {
            get
            {
                return Username + EMAIL_SUFFIX;
            }
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (DepartmentId == 0 && Department == null)
            {
                yield return new ValidationResult
                 ("Employee must be assigned to a department", new[] { "DepartmentId" });
            }
        }
    }
}
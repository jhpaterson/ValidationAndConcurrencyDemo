using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace ValidationAndConcurrencyDemo.Models
{
    public class CompanyContextInitializer : DropCreateDatabaseAlways<CompanyContext>
    {
        protected override void Seed(CompanyContext context)
        {
            base.Seed(context);

            Employee emp1 = new Employee
            {
                Name = "Jenson",
                Username = "jenson",
                Grade = 8,
                PhoneNumber = "9876"
            };

            Employee emp2 = new Employee
            {
                Name = "Checo",
                Username = "checo",
                Grade = 2,
                PhoneNumber = "5432"
            };

            Employee emp3 = new Employee
            {
                Name = "Fernando",
                Username = "fernando",
                Grade = 9,
                PhoneNumber = "1234"
            };

            Employee emp4 = new Employee
            {
                Name = "Felipe",
                Username = "felipe",
                Grade = 4,
                PhoneNumber = "5678"
            };

            Employee emp5 = new Employee
            {
                Name = "Seb",
                Username = "seb",
                Grade = 6,
                PhoneNumber = "1468"
            };

            Department dep1 = new Department
            {
                Name = "Marketing",
                Employees = new List<Employee>{
                    emp1,
                    emp3
                }
            };

            Department dep2 = new Department
            {
                Name = "Sales",
                Employees = new List<Employee>{
                    emp2,
                    emp4,
                    emp5
                }
            };

            context.Departments.Add(dep1);
            context.Departments.Add(dep2);

            context.SaveChanges();
        }
    }
}
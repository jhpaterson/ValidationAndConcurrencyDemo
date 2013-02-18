using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Data;

namespace ValidationAndConcurrencyDemo.Models
{
    public class CompanyContext : DbContext
    {
         public CompanyContext()
            : base()
        {
            Database.SetInitializer<CompanyContext>(new CompanyContextInitializer());
            this.Configuration.LazyLoadingEnabled = false;
            this.Configuration.ProxyCreationEnabled = false;
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Department> Departments { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>()
                .Ignore(e => e.Email);

            modelBuilder.Entity<Employee>()
                .Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(10);

            modelBuilder.Entity<Department>()
               .HasMany(d => d.Employees)
               .WithRequired(e => e.Department);
        }

        protected override DbEntityValidationResult ValidateEntity(
            System.Data.Entity.Infrastructure.DbEntityEntry entityEntry, IDictionary<object, object> items)
        {
            var result = new DbEntityValidationResult(entityEntry, new List<DbValidationError>());

            if (entityEntry.Entity is Employee && entityEntry.State == EntityState.Added)
            {
                Employee emp = entityEntry.Entity as Employee;
                //check for uniqueness of post title
                if (Employees.Where(e => e.Username == emp.Username).Count() > 0)
                    result.ValidationErrors.Add(
                            new System.Data.Entity.Validation.DbValidationError("Username",
                            "this username is not available")
                            );
            }
            if (result.ValidationErrors.Count() > 0)
            {
                return result;
            }
            else
            {
                return base.ValidateEntity(entityEntry, items);
            }
        }
    }
}
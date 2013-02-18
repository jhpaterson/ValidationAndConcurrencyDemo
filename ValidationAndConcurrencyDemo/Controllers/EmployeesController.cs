using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ValidationAndConcurrencyDemo.Models;

namespace ValidationAndConcurrencyDemo.Controllers
{
    public class EmployeesController : Controller
    {
        private CompanyContext db = new CompanyContext();

        //
        // GET: /Employees/

        public ActionResult Index()
        {
            var employees = db.Employees.Include(e => e.Department);
            return View(employees.ToList());
        }

        //
        // GET: /Employees/Details/5

        public ActionResult Details(int id = 0)
        {
            Employee employee = db.Employees.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            return View(employee);
        }

        //
        // GET: /Employees/Create

        public ActionResult Create()
        {
            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "Name");
            return View();
        }

        //
        // POST: /Employees/Create

        // validation works as follows:
        //Username Reuqired, client side from annotation
        //Grade, client side from non-nullable type
        //DepartmentId, client side from non-nullable type
        //Name Required and MaxLength, EF from Fluent
        //Username unique, EF from DbContext ValidateEntity 
        //Department/DepartmentId not null/0, EF from IValidatableObject
        //PhoneNumber, not required
        //if ClientValidationEnabled turned off in web.config then all validation is done by EF
        // note that using selectlist/DropDown for DepartmentId posts null so is validated client-side, using text box allows
        // 0 to be entered to show triggering of IValidatableObject
        [HttpPost]
        public ActionResult Create(Employee employee)
        {
            try
            {
                db.Employees.Add(employee);
                // db.Configuration.ValidateOnSaveEnabled = false;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (DbEntityValidationException ex)
            {
                var error = ex.EntityValidationErrors.First().ValidationErrors.First();
                this.ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                ViewBag.DepartmentId = new SelectList(
                    db.Departments, "DepartmentId", "Name", employee.DepartmentId);
                return View();
            }
        }

        //
        // GET: /Employees/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Employee employee = db.Employees.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "Name", employee.DepartmentId);
            return View(employee);
        }

        //
        // POST: /Employees/Edit/5

        // concurrency exception is thrown if concurrency token in object to be saved does not
        // match token in row of database
        // Employee has a RowVersion property, annotated with [Timestamp], as its concurrency token
        // if editing employee with id = 1, SQL statement will have changed row, database will have updated
        // token, so token in object to save will not match database value, exception will be thrown
        // note that RowVersion is sent as a hidden field by Edit form, will be included in CurrentValues and OriginalValues when
        // attached to context (which happens when state set explicitly to Modified)
        // after exception, timestamp sent by resubmission will match database value (if SQL statement didn't run again...)

        [HttpPost]
        public ActionResult Edit(Employee employee)
        {  
            try
            {
                db.Entry(employee).State = EntityState.Modified;

                // simulate database row being changed by another user
                db.Database.ExecuteSqlCommand(
                    @"UPDATE Employees SET PhoneNumber = 9999, Username = 'changed' WHERE EmployeeId = @Id",
                    new SqlParameter("Id", 1));

                // property values so we can set breakpoint and see what is going on
                var emp1 = ((Employee)db.Entry(employee).CurrentValues.ToObject());
                var emp2 = ((Employee)db.Entry(employee).OriginalValues.ToObject());
                var emp3 = ((Employee)db.Entry(employee).GetDatabaseValues().ToObject());


                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (DbUpdateConcurrencyException ex)
            {
                // note this will not resolve problem here as SQL statement runs every time!
                this.ModelState.AddModelError(string.Empty, "employee data has been changed since you started editing");
                db.Entry(employee).Reload();   // overwrites current values with database values, database wins

                // property values so we can set breakpoint and see what is going on
                var emp1 = ((Employee)db.Entry(employee).CurrentValues.ToObject());
                var emp2 = ((Employee)db.Entry(employee).OriginalValues.ToObject());
                var emp3 = ((Employee)db.Entry(employee).GetDatabaseValues().ToObject());

                ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "Name", employee.DepartmentId);
                return View(employee);    // note raw HTML inputs rather than HTML Helpers in view seems to prevent form caching
            }
        }

        //
        // GET: /Employees/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Employee employee = db.Employees.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            return View(employee);
        }

        //
        // POST: /Employees/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Employee employee = db.Employees.Find(id);
            db.Employees.Remove(employee);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
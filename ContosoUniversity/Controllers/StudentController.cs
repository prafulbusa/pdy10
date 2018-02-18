using System;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using ContosoUniversity.DAL;
using ContosoUniversity.Models;
using PagedList;

namespace ContosoUniversity.Controllers
{
    public class StudentController : Controller
    {
        private SchoolContext db = new SchoolContext();

        // GET: Student
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            // these are both using the ternary operator
            // 
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name-desc" : "";
            ViewBag.DateSortParm = sortOrder == "Date" ? "date-desc" : "Date";

            if(searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var students = from s in db.Students
                           select s;

            // added to do search filtering
            if(!String.IsNullOrEmpty(searchString))
            {
                students = students.Where(s => s.LastName.Contains(searchString)
                                        || s.FirstMidName.Contains(searchString));
            }


            switch(sortOrder)
            {
                case "name-desc":
                    students = students.OrderByDescending(s => s.LastName);
                    break;
                case "Date":
                    students = students.OrderBy(s => s.EnrollmentDate);
                    break;
                case "date-desc":
                    students = students.OrderByDescending(s => s.EnrollmentDate);
                    break;
                default:
                    students = students.OrderBy(s => s.LastName);
                    break;
            }

            int pageSize = 3;
            int pageNumber = (page ?? 1);
            return View(students.ToPagedList(pageNumber, pageSize));
        }

        // GET: Student/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        // GET: Student/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Student/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "LastName,FirstMidName,EmailAddress,EnrollmentDate")] Student student)
        {
            // removed ID from the model binder, because the user does not set ID
            try
            {
                if (ModelState.IsValid)
                {
                    db.Students.Add(student);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (RetryLimitExceededException dex)
            {
                Console.WriteLine(dex.ToString());  // should be logging this better
                // Log the error (uncomment dex var name and add a line here to write a log)
                ModelState.AddModelError("", "Unable to save changes. Try again, and if problem persists see your administrator.");
            }

            return View(student);
        }

        // GET: Student/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        // POST: Student/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]


        public ActionResult EditPost(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var studentToUpdate = db.Students.Find(id);
            if (TryUpdateModel(studentToUpdate, "",
                new string[] { "LastName", "FirstMidName", "EmailAddress", "EnrollmentDate" }))
            {
                try
                {
                    db.SaveChanges();
                }
                catch (RetryLimitExceededException dex)
                {
                    Console.WriteLine(dex.ToString());  // should be logging this better
                                                        // Log the error (uncomment dex var name and add a line here to write a log)
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if problem persists see your administrator.");
                }
            }
            return View(studentToUpdate);
        }

        // GET: Student/Delete/5
        public ActionResult Delete(int? id, bool? saveChangesError=false)
        {
            // accepts an optional parameter that indicates if this method
            // was called after a failure to save changes - param is false
            // when called without a previous failure
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if(saveChangesError.GetValueOrDefault())
            {
                ViewBag.ErrorMessage = "Delete failed. Try again and if the problem persists see your system administrator.";
            }
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        // POST: Student/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            // this POST delete performs the actual delete operation
            // and catches any db update errors 
            try
            {
                Student student = db.Students.Find(id);
                db.Students.Remove(student);
                db.SaveChanges();

                // or for high performance app - this is all the EF needs to 
                // delete an entry
                //Student studentToDelete = new Student() { ID = id };
                //db.Entry(studentToDelete).State = EntityState.Deleted;

            }
            catch (RetryLimitExceededException dex)
            {
                // the retrylimitexceeded exception strategy is when the only errors
                // likely to be transient will already have been tried and failed
                // several times and the actual exception returned will be wrapped in the 
                // RetryLimitExceededException object.
                Console.WriteLine(dex.ToString());  // should be logging this better
                // Log the error (uncomment dex var name and add a line here to write a log)
                ModelState.AddModelError("", "Unable to save changes. Try again, and if problem persists see your administrator.");
            }
            catch (DataException dex)
            {
                // the data exception gives a friendly "try again" strategy
                Console.WriteLine(dex.ToString());  // should be logging this better
                                                    // Log the error (uncomment dex var name and add a line here to write a log)
                return RedirectToAction("Delete", new { id = id, saveChangesError = true });
            }
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            // critical performance method releases resources as soon as possible
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

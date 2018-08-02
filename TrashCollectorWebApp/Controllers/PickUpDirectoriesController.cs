using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TrashCollectorWebApp.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace TrashCollectorWebApp.Controllers
{
    public class PickUpDirectoriesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: PickUpDirectories
        public ActionResult Index()
        {
            string userId = User.Identity.GetUserId();
            if (User.IsInRole("Customer"))
            {
                return RedirectToAction("ListCustomer"); 
            }
            else if (User.IsInRole("Employee"))
            {
                return RedirectToAction("ListEmployee");
            }
            return RedirectToAction("Index", "Home");
        }
        // GET: PickUpDirectories FOR Employee Role
        public ViewResult ListEmployee(string day)
        {
            string userId = User.Identity.GetUserId();
            Employee currentEmployee = db.Employees.Where(x => x.UserId == userId).Select(x => x).FirstOrDefault();
            DateTime today = DateTime.Today;
            string dayOfWeek = today.DayOfWeek.ToString();
            List<string> daysOfWeek = new List<string>() { "Today", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday", "All" };
            ViewBag.DaysOfWeek = new SelectList(daysOfWeek);
            if(day == "Today")
            {
                day = dayOfWeek;
            }

            var pickUps = db.PickUpDirectories.Where(x => x.Customer.ZipCode == currentEmployee.AssignedZip).Where(x => x.DayOfWeek == dayOfWeek).Select(x=>x);
            if (!string.IsNullOrEmpty(day)) 
            {
                if(day.ToLower() == "all")
                {
                    pickUps = db.PickUpDirectories.Where(x => x.Customer.ZipCode == currentEmployee.AssignedZip);
                }
                else
                {
                    pickUps = db.PickUpDirectories.Where(x => x.Customer.ZipCode == currentEmployee.AssignedZip && x.DayOfWeek.ToLower() == day.ToLower());
                }
            }
            SetActiveInactiveStatus(pickUps.ToList());
            pickUps = pickUps.Where(x=>x.IsActive == true).Include(t => t.Customer);
            return View(pickUps.ToList());
        }
        public ActionResult ListCustomer()
        {
            string userId = User.Identity.GetUserId();
            Customer currentCustomer = db.Customers.Where(x => x.UserId == userId).Select(x => x).FirstOrDefault();
            var pickUpDirectories = db.PickUpDirectories.Where(x => x.CustomerID == currentCustomer.ID).Include(t => t.Customer);
            return View(pickUpDirectories.ToList());
        }
        public void SetActiveInactiveStatus(List<PickUpDirectory> directories)
        {
            foreach(PickUpDirectory item in directories)
            {
                item.IsActive = IsActiveDate(item);
                db.SaveChanges();
            }
        }
        public bool IsActiveDate(PickUpDirectory directory)
        {
            DateTime today = DateTime.Today;
            string dtString = today.ToString("MM/dd/yyyy");
            int[] dtFormatted = dtString.Split('/').Select(x => Int32.Parse(x)).ToArray();
            int[] startDateDtFormatted = directory.StartDate.Split('/').Select(x => Int32.Parse(x)).ToArray();
            int[] EndDateDtFormatted = directory.EndDate.Split('/').Select(x => Int32.Parse(x)).ToArray();
            bool isActive = (dtFormatted[0] >= startDateDtFormatted[0] && dtFormatted[0] <= EndDateDtFormatted[0] && dtFormatted[2] >= startDateDtFormatted[2] && dtFormatted[2] <= EndDateDtFormatted[2]) ? true : false;
            if(dtFormatted[0] == EndDateDtFormatted[0])
            {
                isActive = (dtFormatted[1] <= EndDateDtFormatted[1] && dtFormatted[1] >= startDateDtFormatted[1] && isActive == true) ? true : false;
            }
            return isActive;
        }

        // GET: PickUpDirectories/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PickUpDirectory pickUpDirectory = db.PickUpDirectories.Find(id);
            if (pickUpDirectory == null)
            {
                return HttpNotFound();
            }
            return View(pickUpDirectory);
        }

        // GET: PickUpDirectories/Create
        public ActionResult Create()
        {
            // ViewBag.CustomerID = new SelectList(db.Customers, "ID", "FirstName");
            List<string> daysOfWeek = new List<string>() { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };
            ViewBag.DaysOfWeek = new SelectList(daysOfWeek);
            return View();
        }

        // POST: PickUpDirectories/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,DayOfWeek,SpecialPickUp,SpecialDate,PickUpCompleted,StartDate,EndDate")] PickUpDirectory pickUpDirectory)
        {
            if (ModelState.IsValid)
            {
                string userId = User.Identity.GetUserId();
                Customer currentCustomer = db.Customers.Where(x => x.UserId == userId).Select(x => x).FirstOrDefault();
                pickUpDirectory.CustomerID = currentCustomer.ID;
                db.PickUpDirectories.Add(pickUpDirectory);
                db.SaveChanges();

                // Create Transaction
                //Transaction newTransaction = new Transaction();
                //newTransaction.CustomerID = pickUpDirectory.CustomerID;
                //newTransaction.Amount = (pickUpDirectory.SpecialPickUp) ? 50 : 25;
                //db.Transactions.Add(newTransaction);
                //db.SaveChanges();
                if (User.IsInRole("Customer"))
                {
                    return RedirectToAction("Index", "Customers");
                }
                else if (User.IsInRole("Employee"))
                {
                    return RedirectToAction("Index", "Employees");
                }
                
            }

            ViewBag.CustomerID = new SelectList(db.Customers, "ID", "FirstName", pickUpDirectory.CustomerID);
            return View(pickUpDirectory);
        }

        // GET: PickUpDirectories/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PickUpDirectory pickUpDirectory = db.PickUpDirectories.Find(id);
            if (pickUpDirectory == null)
            {
                return HttpNotFound();
            }
            ViewBag.CustomerID = new SelectList(db.Customers, "ID", "FirstName", pickUpDirectory.CustomerID);
            return View(pickUpDirectory);
        }

        // POST: PickUpDirectories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,DayOfWeek,SpecialPickUp,SpecialDate,PickUpCompleted,CustomerID")] PickUpDirectory pickUpDirectory)
        {
            if (ModelState.IsValid)
            {
                db.Entry(pickUpDirectory).State = EntityState.Modified;
                db.SaveChanges();
                
                if (pickUpDirectory.PickUpCompleted)
                {
                    return RedirectToAction("Create", "Transactions");
                }
                return RedirectToAction("Index");
            }
            ViewBag.CustomerID = new SelectList(db.Customers, "ID", "FirstName", pickUpDirectory.CustomerID);
            return View(pickUpDirectory);
        }

        // GET: PickUpDirectories/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PickUpDirectory pickUpDirectory = db.PickUpDirectories.Find(id);
            if (pickUpDirectory == null)
            {
                return HttpNotFound();
            }
            return View(pickUpDirectory);
        }

        // POST: PickUpDirectories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            PickUpDirectory pickUpDirectory = db.PickUpDirectories.Find(id);
            db.PickUpDirectories.Remove(pickUpDirectory);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

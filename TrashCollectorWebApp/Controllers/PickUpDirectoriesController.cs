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
                Customer currentCustomer = db.Customers.Where(x => x.UserId == userId).Select(x => x).FirstOrDefault();
                var pickUpDirectories = db.PickUpDirectories.Where(x => x.CustomerID == currentCustomer.ID);
                return View(pickUpDirectories.ToList());
            }
            else if (User.IsInRole("Employee"))
            {
                Employee currentEmployee = db.Employees.Where(x => x.UserId == userId).Select(x => x).FirstOrDefault();
                var requiredData =
                    from x in db.PickUpDirectories
                    where x.Customer.ZipCode == currentEmployee.AssignedZip
                    select x;
                //var pickUpDirectories = db.PickUpDirectories.Where(x => x.Customer.ZipCode == currentEmployee.AssignedZip);
                return View(requiredData.ToList());
            }
            return RedirectToAction("Index", "Home");
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
        public ActionResult Create([Bind(Include = "ID,DayOfWeek,SpecialPickUp,SpecialDate,PickUpCompleted")] PickUpDirectory pickUpDirectory)
        {
            if (ModelState.IsValid)
            {
                string userId = User.Identity.GetUserId();
                Customer currentCustomer = db.Customers.Where(x => x.UserId == userId).Select(x => x).FirstOrDefault();
                pickUpDirectory.CustomerID = currentCustomer.ID;
                db.PickUpDirectories.Add(pickUpDirectory);
                db.SaveChanges();
                return RedirectToAction("Index", "Customers");
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

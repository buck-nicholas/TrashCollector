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
using GoogleMaps.LocationServices;


namespace TrashCollectorWebApp.Controllers
{
    public class EmployeesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Employees
        public ActionResult Index()
        {
            string userID = User.Identity.GetUserId();
            var currentUser = db.Employees.Where(x => x.UserId == userID).Select(x => x).FirstOrDefault();
            return View(currentUser);
        }

        // GET: Employees/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = db.Employees.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            return View(employee);
        }

        // GET: Employees/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Employees/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,FirstName,LastName,AssignedZip")] Employee employee)
        {
            string currentUserId = User.Identity.GetUserId();
            if (ModelState.IsValid)
            {
                employee.UserId = currentUserId;
                db.Employees.Add(employee);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(employee);
        }

        // GET: Employees/Edit/5
        public ActionResult Edit()
        {
            var userID = User.Identity.GetUserId();
            int? id = (User.IsInRole("Employee")) ? db.Employees.Where(x => x.UserId == userID).Select(x => x.ID).FirstOrDefault() : 0;

            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = db.Employees.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            return View(employee);
        }

        // POST: Employees/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,FirstName,LastName,UserId,AssignedZip")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                db.Entry(employee).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(employee);
        }

        // GET: Employees/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = db.Employees.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            return View(employee);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Employee employee = db.Employees.Find(id);
            db.Employees.Remove(employee);
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

        public ViewResult CustomerLocations(string selectedID)
        {
            //var requiredData = db.Customers.Select(x => x);
            var requiredData =
                (from x in db.Customers
                 select new SelectListItem
                 {
                     Text = x.FirstName.ToString(),
                     Value = x.ID.ToString()
                 });
            ViewData["CustList"] = requiredData.ToList();
            //ViewBag.CustomerList = new SelectList(requiredData.ToList());

            if(!string.IsNullOrEmpty(selectedID))
            {
                var id = Int32.Parse(selectedID);
                Customer customer = db.Customers.Where(x => x.ID == id).Select(x => x).First();
                string addressLineOneFormated = customer.AddressLineOne.Replace(' ', '+');
                string addressLineTwoFormated = (!string.IsNullOrEmpty(customer.AddressLineTwo)) ? customer.AddressLineTwo.Replace(' ', '+') : null;
                string streetAddress = (!string.IsNullOrEmpty(addressLineTwoFormated)) ? addressLineOneFormated + "+" + addressLineTwoFormated : addressLineOneFormated;
                string formattedCity = customer.City.Replace(' ', '+');
                string formattedAddressComplete = streetAddress + "," + formattedCity + "," + customer.USState + "," + customer.ZipCode.ToString();
                ViewBag.Key = ApiKey.key;
                ViewBag.Address = formattedAddressComplete;

                var testAddress = customer.AddressLineOne + " " + ((!string.IsNullOrEmpty(customer.AddressLineTwo)) ? customer.AddressLineTwo + " " : "") + customer.City + ", " + customer.USState + " " + customer.ZipCode;
                var locationService = new GoogleLocationService();
                var point = locationService.GetLatLongFromAddress(testAddress);
                var latitude = point.Latitude;
                var longitude = point.Longitude;
                ViewBag.Point = point;
                ViewBag.Lat = latitude;
                ViewBag.Long = longitude;
            }
            return View();
        }
    }
}

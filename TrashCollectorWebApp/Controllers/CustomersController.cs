﻿using System;
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
    public class CustomersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Customers
        public ActionResult Index()
        {
            string userID = User.Identity.GetUserId();
            var currentUser = db.Customers.Where(x => x.UserId == userID).Select(x=>x).FirstOrDefault();
            return View(currentUser);
        }

        // GET: Customers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        // GET: Customers/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Customers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,FirstName,LastName,AddressLineOne,AddressLineTwo,City,USState,ZipCode")] Customer customer)
        {
            string currentUserId = User.Identity.GetUserId();
            // ApplicationUser newUser = new ApplicationUser();
            
            if (ModelState.IsValid)
            {
                customer.UserId = currentUserId;
                db.Customers.Add(customer);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(customer);
        }

        // GET: Customers/Edit/5
        public ActionResult Edit()
        {
            var userID = User.Identity.GetUserId();
            int? id = (User.IsInRole("Customer")) ? db.Customers.Where(x => x.UserId == userID).Select(x => x.ID).FirstOrDefault() : 0;

            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        // POST: Customers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,FirstName,LastName,AddressLineOne,AddressLineTwo,City,USState,ZipCode,UserId")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                db.Entry(customer).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(customer);
        }

        // GET: Customers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Customer customer = db.Customers.Find(id);
            db.Customers.Remove(customer);
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

        public ActionResult MappedLocation(int id)
        {
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

            return View();
        }

    }
}

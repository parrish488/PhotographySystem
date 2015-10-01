using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PhotgraphyMVC.Models;

namespace PhotgraphyMVC.Controllers
{
    [Authorize]
    public class MileagesController : Controller
    {
        private PhotographerContext db = new PhotographerContext();

        // GET: Mileages
        public ActionResult Index()
        {
            var mileage = db.Mileage.Include(m => m.Client).Include(m => m.ClientEvent);
            return View(mileage.ToList());
        }

        // GET: Mileages/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Mileage mileage = db.Mileage.Find(id);
            if (mileage == null)
            {
                return HttpNotFound();
            }
            return View(mileage);
        }

        // GET: Mileages/Create
        public ActionResult Create()
        {
            ViewBag.ClientID = new SelectList(db.Clients, "ClientID", "FullName");
            ViewBag.EventID = new SelectList(db.Events, "EventID", "EventLabel");
            return View();
        }

        // POST: Mileages/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MileageID,ClientID,EventID,MilesDriven,Source")] Mileage mileage)
        {
            if (ModelState.IsValid)
            {
                db.Mileage.Add(mileage);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ClientID = new SelectList(db.Clients, "ClientID", "FullName", mileage.ClientID);
            ViewBag.EventID = new SelectList(db.Events, "EventID", "EventLabel", mileage.EventID);
            return View(mileage);
        }

        // GET: Mileages/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Mileage mileage = db.Mileage.Find(id);
            if (mileage == null)
            {
                return HttpNotFound();
            }
            ViewBag.ClientID = new SelectList(db.Clients, "ClientID", "FullName", mileage.ClientID);
            ViewBag.EventID = new SelectList(db.Events, "EventID", "EventLabel", mileage.EventID);
            return View(mileage);
        }

        // POST: Mileages/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MileageID,ClientID,EventID,MilesDriven,Source")] Mileage mileage)
        {
            if (ModelState.IsValid)
            {
                db.Entry(mileage).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ClientID = new SelectList(db.Clients, "ClientID", "FullName", mileage.ClientID);
            ViewBag.EventID = new SelectList(db.Events, "EventID", "EventLabel", mileage.EventID);
            return View(mileage);
        }

        // GET: Mileages/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Mileage mileage = db.Mileage.Find(id);
            if (mileage == null)
            {
                return HttpNotFound();
            }
            return View(mileage);
        }

        // POST: Mileages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Mileage mileage = db.Mileage.Find(id);
            db.Mileage.Remove(mileage);
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

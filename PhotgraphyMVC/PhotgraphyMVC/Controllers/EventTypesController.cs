using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace PhotgraphyMVC.Models
{
    public class EventTypesController : Controller
    {
        private PhotographerContext db = new PhotographerContext();

        // GET: EventTypes
        public ActionResult Index()
        {
            return View(db.EventTypes.Where(e => e.Username == User.Identity.Name).ToList());
        }

        // GET: EventTypes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EventTypes eventTypes = db.EventTypes.Find(id);
            if (eventTypes == null)
            {
                return HttpNotFound();
            }
            return View(eventTypes);
        }

        // GET: EventTypes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: EventTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,EventTypeName,Username")] EventTypes eventTypes)
        {
            if (ModelState.IsValid)
            {
                eventTypes.Username = User.Identity.Name;
                db.EventTypes.Add(eventTypes);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(eventTypes);
        }

        // GET: EventTypes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EventTypes eventTypes = db.EventTypes.Find(id);
            if (eventTypes == null)
            {
                return HttpNotFound();
            }
            return View(eventTypes);
        }

        // POST: EventTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,EventTypeName,Username")] EventTypes eventTypes)
        {
            if (ModelState.IsValid)
            {
                eventTypes.Username = User.Identity.Name;
                db.Entry(eventTypes).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(eventTypes);
        }

        // GET: EventTypes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EventTypes eventTypes = db.EventTypes.Find(id);
            if (eventTypes == null)
            {
                return HttpNotFound();
            }
            return View(eventTypes);
        }

        // POST: EventTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            EventTypes eventTypes = db.EventTypes.Find(id);
            db.EventTypes.Remove(eventTypes);
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

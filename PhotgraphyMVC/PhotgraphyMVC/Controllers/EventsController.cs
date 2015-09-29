﻿using System;
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
    public class EventsController : Controller
    {
        private PhotographerContext db = new PhotographerContext();

        // GET: Events
        public ActionResult Index(string sortOrder)
        {
            ViewBag.LastNameSortParm = String.IsNullOrEmpty(sortOrder) ? "last_name_desc" : "";
            ViewBag.FirstNameSortParm = String.IsNullOrEmpty(sortOrder) ? "first_name_desc" : "first_name";
            ViewBag.EventTypeSortParm = String.IsNullOrEmpty(sortOrder) ? "event_type_desc" : "event_type";
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";

            List<Event> events = new List<Event>();

            switch (sortOrder)
            {
                case "event_type":
                    events = db.Events.Include(c => c.Client).OrderBy(c => c.EventType).ToList();
                    break;
                case "event_type_desc":
                    events = db.Events.Include(c => c.Client).OrderByDescending(c => c.EventType).ToList();
                    break;                
                case "Date":
                    events = db.Events.Include(c => c.Client).OrderBy(c => c.EventDate).ToList();
                    break;
                case "date_desc":
                    events = db.Events.Include(c => c.Client).OrderByDescending(c => c.EventDate).ToList();
                    break;
                case "first_name":
                    events = db.Events.Include(c => c.Client).OrderBy(c => c.Client.FirstName).ToList();
                    break;
                case "first_name_desc":
                    events = db.Events.Include(c => c.Client).OrderByDescending(c => c.Client.FirstName).ToList();
                    break;
                case "last_name_desc":
                    events = db.Events.Include(c => c.Client).OrderBy(c => c.Client.LastName).ToList();
                    break;
                default:
                    events = db.Events.Include(c => c.Client).OrderByDescending(c => c.Client.LastName).ToList();
                    break;
            }
            
            return View(events);
        }

        // GET: Events/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Event @event = db.Events.Find(id);
            if (@event == null)
            {
                return HttpNotFound();
            }
            return View(@event);
        }

        // GET: Events/Create
        public ActionResult Create()
        {
            ViewBag.ClientID = new SelectList(db.Clients, "ClientID", "FullName");
            return View();
        }

        // POST: Events/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "EventID,EventDate,EventType,ClientID")] Event @event)
        {
            if (ModelState.IsValid)
            {
                db.Events.Add(@event);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ClientID = new SelectList(db.Clients, "ClientID", "FullName", @event.ClientID);
            return View(@event);
        }

        // GET: Events/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Event @event = db.Events.Find(id);
            if (@event == null)
            {
                return HttpNotFound();
            }
            ViewBag.ClientID = new SelectList(db.Clients, "ClientID", "FullName", @event.ClientID);
            return View(@event);
        }

        // POST: Events/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "EventID,EventDate,EventType,ClientID")] Event @event)
        {
            if (ModelState.IsValid)
            {
                db.Entry(@event).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ClientID = new SelectList(db.Clients, "ClientID", "FullName", @event.ClientID);
            return View(@event);
        }

        // GET: Events/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Event @event = db.Events.Find(id);
            if (@event == null)
            {
                return HttpNotFound();
            }
            return View(@event);
        }

        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Event @event = db.Events.Find(id);
            db.Events.Remove(@event);
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

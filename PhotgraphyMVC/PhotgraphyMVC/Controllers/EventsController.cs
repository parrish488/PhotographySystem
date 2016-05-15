using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PhotgraphyMVC.Models;
using PagedList;

namespace PhotgraphyMVC.Controllers
{
    [Authorize]
    public class EventsController : Controller
    {
        private PhotographerContext db = new PhotographerContext();

        // GET: Events
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.LastNameSortParm = String.IsNullOrEmpty(sortOrder) ? "date_desc" : "";
            ViewBag.FirstNameSortParm = sortOrder == "first_name" ? "first_name_desc" : "first_name";
            ViewBag.EventTypeSortParm = sortOrder == "event_type" ? "event_type_desc" : "event_type";
            ViewBag.DateSortParm = sortOrder == "last_name" ? "last_name_desc" : "last_name";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var events = from e in db.Events
                        select e;
            if (!String.IsNullOrEmpty(searchString))
            {
                events = events.Where(e => e.Client.LastName.Contains(searchString)
                                       || e.Client.FirstName.Contains(searchString)
                                       || e.EventType.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "event_type":
                    events = events.Include(c => c.Client).OrderBy(c => c.EventType);
                    break;
                case "event_type_desc":
                    events = events.Include(c => c.Client).OrderByDescending(c => c.EventType);
                    break;                
                case "first_name":
                    events = events.Include(c => c.Client).OrderBy(c => c.Client.FirstName);
                    break;
                case "first_name_desc":
                    events = events.Include(c => c.Client).OrderByDescending(c => c.Client.FirstName);
                    break;
                case "last_name_desc":
                    events = events.Include(c => c.Client).OrderByDescending(c => c.Client.LastName);
                    break;
                case "last_name":
                    events = events.Include(c => c.Client).OrderBy(c => c.Client.LastName);
                    break;
                case "date_desc":
                    events = events.Include(c => c.Client).OrderByDescending(c => c.EventDate);
                    break;
                default:
                    events = events.Include(c => c.Client).OrderBy(c => c.EventDate);                    
                    break;
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(events.ToPagedList(pageNumber, pageSize));
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
        public ActionResult Create([Bind(Include = "EventID,EventDate,EventType,ContractCompleted,ClientID")] Event @event)
        {
            if (ModelState.IsValid)
            {
                db.Events.Add(@event);
                db.SaveChanges();

                HomeController.VerifyActiveStatus(db);

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
        public ActionResult Edit([Bind(Include = "EventID,EventDate,EventType,ContractCompleted,ClientID")] Event @event)
        {
            if (ModelState.IsValid)
            {
                db.Entry(@event).State = EntityState.Modified;
                db.SaveChanges();

                HomeController.VerifyActiveStatus(db);

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

            HomeController.VerifyActiveStatus(db);

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

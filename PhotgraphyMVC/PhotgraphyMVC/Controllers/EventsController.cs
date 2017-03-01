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
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page, int? eventYearSelection)
        {
            ViewBag.LastNameSortParm = string.IsNullOrEmpty(sortOrder) ? "last_name" : "";
            ViewBag.FirstNameSortParm = sortOrder == "first_name" ? "first_name_desc" : "first_name";
            ViewBag.EventTypeSortParm = sortOrder == "event_type" ? "event_type_desc" : "event_type";
            ViewBag.DateSortParm = sortOrder == "date" ? "date_desc" : "date";
            ViewBag.EventYearSelection = eventYearSelection;

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var eventYears = (from e in db.Events
                              where e.Username == User.Identity.Name
                              select e.EventDate.Year).Distinct();

            ViewBag.EventYears = eventYears.OrderByDescending(e => e).ToList();

            var events = from e in db.Events
                         where e.Username == User.Identity.Name
                         select e;

            if (!string.IsNullOrEmpty(searchString))
            {
                events = events.Where(e => e.Client.LastName.Contains(searchString)
                                       || e.Client.FirstName.Contains(searchString)
                                       || e.EventType.Contains(searchString));
            }

            if (eventYearSelection != null && eventYearSelection != 0)
            {
                events = events.Where(e => e.EventDate.Year == eventYearSelection);
            }
            else if (eventYearSelection == null)
            {
                events = events.Where(e => e.EventDate.Year == DateTime.Now.Year);
                ViewBag.EventYearSelection = DateTime.Now.Year;
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
            var clients = from c in db.Clients
                          where c.Username == User.Identity.Name
                          orderby c.LastName ascending
                          select c;

            var eventTypes = from e in db.EventTypes
                          where e.Username == User.Identity.Name
                          orderby e.EventTypeName ascending
                          select e;

            ViewBag.ClientID = new SelectList(clients, "ClientID", "FullName");
            ViewBag.EventTypes = new SelectList(eventTypes, "ID", "EventTypeName");

            return View();
        }

        // POST: Events/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "EventID,EventDate,EventType,ContractCompleted,Username,EventTypeID,ClientID")] Event @event)
        {
            if (ModelState.IsValid)
            {
                @event.Username = User.Identity.Name;
                @event.EventType = db.EventTypes.Find(@event.EventTypeID).EventTypeName;
                db.Events.Add(@event);

                HomeController.VerifyActiveStatus(db, User.Identity.Name);

                return RedirectToAction("Index");
            }

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

            var clients = from c in db.Clients
                          where c.Username == User.Identity.Name
                          orderby c.LastName ascending
                          select c;

            var eventTypes = from e in db.EventTypes
                             where e.Username == User.Identity.Name
                             orderby e.EventTypeName ascending
                             select e;

            @event.ClientIDs = clients.ToList();
            @event.EventTypeIDs = eventTypes.ToList();

            return View(@event);
        }

        // POST: Events/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "EventID,EventDate,EventType,ContractCompleted,Username,EventTypeID,ClientID")] Event @event)
        {
            if (ModelState.IsValid)
            {
                @event.Username = User.Identity.Name;
                @event.EventType = db.EventTypes.Find(@event.EventTypeID).EventTypeName;

                db.Entry(@event).State = EntityState.Modified;

                HomeController.VerifyActiveStatus(db, User.Identity.Name);

                return RedirectToAction("Index");
            }

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

            HomeController.VerifyActiveStatus(db, User.Identity.Name);

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

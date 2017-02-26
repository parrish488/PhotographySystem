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
using Newtonsoft.Json;

namespace PhotgraphyMVC.Controllers
{
    [Authorize]
    public class EventsController : Controller
    {
        private const string apiUrl = "http://localhost:57669/";

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

            string responseString = Communication.GetRequest(apiUrl, "api/Events/Years", User.Identity.Name);
            ViewBag.EventYears = JsonConvert.DeserializeObject<List<int>>(responseString);

            responseString = Communication.GetRequest(apiUrl, "api/Events", User.Identity.Name);
            var events = JsonConvert.DeserializeObject<IEnumerable<Event>>(responseString);

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
                    events = events.OrderBy(c => c.EventType);
                    break;
                case "event_type_desc":
                    events = events.OrderByDescending(c => c.EventType);
                    break;                
                case "first_name":
                    events = events.OrderBy(c => c.Client.FirstName);
                    break;
                case "first_name_desc":
                    events = events.OrderByDescending(c => c.Client.FirstName);
                    break;
                case "last_name_desc":
                    events = events.OrderByDescending(c => c.Client.LastName);
                    break;
                case "last_name":
                    events = events.OrderBy(c => c.Client.LastName);
                    break;
                case "date_desc":
                    events = events.OrderByDescending(c => c.EventDate);
                    break;
                default:
                    events = events.OrderBy(c => c.EventDate);
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

            string responseString = Communication.GetRequest(apiUrl, "api/Events/" + id, User.Identity.Name);
            Event @event = JsonConvert.DeserializeObject<Event>(responseString);

            return View(@event);
        }

        // GET: Events/Create
        public ActionResult Create()
        {
            string responseString = Communication.GetRequest(apiUrl, "api/Clients", User.Identity.Name);
            var clients = JsonConvert.DeserializeObject<IEnumerable<Client>>(responseString);

            responseString = Communication.GetRequest(apiUrl, "api/EventTypes", User.Identity.Name);
            var eventTypes = JsonConvert.DeserializeObject<IEnumerable<EventTypes>>(responseString);

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
                string responseString = Communication.PostRequest(apiUrl, "api/Events", User.Identity.Name, JsonConvert.SerializeObject(@event));

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

            string responseString = Communication.GetRequest(apiUrl, "api/Events/" + id, User.Identity.Name);
            Event @event = JsonConvert.DeserializeObject<Event>(responseString);

            responseString = Communication.GetRequest(apiUrl, "api/Clients", User.Identity.Name);
            var clients = JsonConvert.DeserializeObject<IEnumerable<Client>>(responseString);

            responseString = Communication.GetRequest(apiUrl, "api/EventTypes", User.Identity.Name);
            var eventTypes = JsonConvert.DeserializeObject<IEnumerable<EventTypes>>(responseString);

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
                string responseString = Communication.PutRequest(apiUrl, "api/Events/" + @event.EventID, User.Identity.Name, JsonConvert.SerializeObject(@event));

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

            string responseString = Communication.GetRequest(apiUrl, "api/Events/" + id, User.Identity.Name);
            Event @event = JsonConvert.DeserializeObject<Event>(responseString);

            return View(@event);
        }

        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            string responseString = Communication.DeleteRequest(apiUrl, "api/Events/" + id, User.Identity.Name);
            Event @event = JsonConvert.DeserializeObject<Event>(responseString);

            return RedirectToAction("Index");
        }
    }
}

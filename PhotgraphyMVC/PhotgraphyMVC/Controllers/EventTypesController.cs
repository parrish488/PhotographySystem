using Newtonsoft.Json;
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
    [Authorize]
    public class EventTypesController : Controller
    {
        // GET: EventTypes
        public ActionResult Index()
        {
            string responseString = Communication.GetRequest("api/EventTypes", User.Identity.Name);
            var eventTypes = JsonConvert.DeserializeObject<IEnumerable<EventTypes>>(responseString);

            return View(eventTypes);
        }

        // GET: EventTypes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            string responseString = Communication.GetRequest("api/EventTypes/" + id, User.Identity.Name);
            EventTypes eventTypes = JsonConvert.DeserializeObject<EventTypes>(responseString);

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
                string responseString = Communication.PostRequest("api/EventTypes", User.Identity.Name, JsonConvert.SerializeObject(eventTypes));

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

            string responseString = Communication.GetRequest("api/EventTypes/" + id, User.Identity.Name);
            EventTypes eventTypes = JsonConvert.DeserializeObject<EventTypes>(responseString);

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
                string responseString = Communication.PutRequest("api/EventTypes/" + eventTypes.ID, User.Identity.Name, JsonConvert.SerializeObject(eventTypes));

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

            string responseString = Communication.GetRequest("api/EventTypes/" + id, User.Identity.Name);
            EventTypes eventTypes = JsonConvert.DeserializeObject<EventTypes>(responseString);

            return View(eventTypes);
        }

        // POST: EventTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            string responseString = Communication.DeleteRequest("api/EventTypes/" + id, User.Identity.Name);
            EventTypes eventTypes = JsonConvert.DeserializeObject<EventTypes>(responseString);

            return RedirectToAction("Index");
        }
    }
}

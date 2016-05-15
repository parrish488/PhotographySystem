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
    public class ClientsController : Controller
    {
        private PhotographerContext db = new PhotographerContext();

        // GET: Clients
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.LastNameSortParm = string.IsNullOrEmpty(sortOrder) ? "last_name_desc" : "";
            ViewBag.FirstNameSortParm = sortOrder == "first_name" ? "first_name_desc" : "first_name";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var clientList = from c in db.Clients select c;

            //foreach (Client client in clientList)
            //{
            //    client.Status = VerifyActiveStatus(client.Events) ? "Active" : "Inactive";
            //}

            if (!string.IsNullOrEmpty(searchString))
            {
                clientList = clientList.Where(c => c.LastName.Contains(searchString)
                                       || c.FirstName.Contains(searchString));
            }

            clientList = clientList.Where(c => c.Status == "Active");

            switch (sortOrder)
            {
                case "first_name":
                    clientList = clientList.OrderBy(c => c.FirstName);
                    break;
                case "first_name_desc":
                    clientList = clientList.OrderByDescending(c => c.FirstName);
                    break;
                case "last_name_desc":
                    clientList = clientList.OrderByDescending(c => c.LastName);
                    break;
                default:
                    clientList = clientList.OrderBy(c => c.LastName);
                    break;
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(clientList.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult CompleteIndex(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.LastNameSortParm = string.IsNullOrEmpty(sortOrder) ? "last_name_desc" : "";
            ViewBag.FirstNameSortParm = sortOrder == "first_name" ? "first_name_desc" : "first_name";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var clientList = from c in db.Clients select c;

            //foreach (Client client in clientList)
            //{
            //    client.Status = VerifyActiveStatus(client.Events) ? "Active" : "Inactive";
            //}

            if (!string.IsNullOrEmpty(searchString))
            {
                clientList = clientList.Where(c => c.LastName.Contains(searchString)
                                       || c.FirstName.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "first_name":
                    clientList = clientList.OrderBy(c => c.FirstName);
                    break;
                case "first_name_desc":
                    clientList = clientList.OrderByDescending(c => c.FirstName);
                    break;
                case "last_name_desc":
                    clientList = clientList.OrderByDescending(c => c.LastName);
                    break;
                default:
                    clientList = clientList.OrderBy(c => c.LastName);
                    break;
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(clientList.ToPagedList(pageNumber, pageSize));
        }

        // GET: Clients/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Client client = db.Clients.Find(id);
            if (client == null)
            {
                return HttpNotFound();
            }
            return View(client);
        }

        // GET: Clients/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Clients/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ClientID,FirstName,LastName,Street,City,State,Zip,PrimaryPhone,SecondaryPhone,Email,ClientNotes,Status")] Client client)
        {
            if (ModelState.IsValid)
            {
                //foreach (Event clientEvent in client.Events)
                //{
                //    if (clientEvent.EventDate >= DateTime.Now.AddMonths(-1))
                //    {
                //        client.Status = "Active";
                //        break;
                //    }
                //    else
                //    {
                //        client.Status = "Inactive";
                //    }
                //}

                db.Clients.Add(client);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(client);
        }

        // GET: Clients/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Client client = db.Clients.Find(id);
            if (client == null)
            {
                return HttpNotFound();
            }
            return View(client);
        }

        // POST: Clients/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ClientID,FirstName,LastName,Street,City,State,Zip,PrimaryPhone,SecondaryPhone,Email,ClientNotes")] Client client)
        {
            if (ModelState.IsValid)
            {
                //foreach (Event clientEvent in client.Events)
                //{
                //    if (clientEvent.EventDate >= DateTime.Now.AddMonths(-1))
                //    {
                //        client.Status = "Active";
                //        break;
                //    }
                //    else
                //    {
                //        client.Status = "Inactive";
                //    }
                //}

                db.Entry(client).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(client);
        }

        // GET: Clients/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Client client = db.Clients.Find(id);
            if (client == null)
            {
                return HttpNotFound();
            }
            return View(client);
        }

        // POST: Clients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Client client = db.Clients.Find(id);
            db.Clients.Remove(client);
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

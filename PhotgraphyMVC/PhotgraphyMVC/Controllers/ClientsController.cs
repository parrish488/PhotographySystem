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
    public class ClientsController : Controller
    {
        private const string apiUrl = "http://localhost:57669/";

        // GET: Clients
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page, bool? activeOnly)
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

            string responseString = Communication.GetRequest(apiUrl, "api/Clients", User.Identity.Name);
            var clientList = JsonConvert.DeserializeObject<IEnumerable<Client>>(responseString);

            if (!string.IsNullOrEmpty(searchString))
            {
                clientList = clientList.Where(c => c.LastName.Contains(searchString)
                                       || c.FirstName.Contains(searchString));
            }

            if (activeOnly != null && activeOnly.Value)
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

        // GET: Clients/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            string responseString = Communication.GetRequest(apiUrl, "api/Clients/" + id, User.Identity.Name);
            Client client = JsonConvert.DeserializeObject<Client>(responseString);

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
        public ActionResult Create([Bind(Include = "ClientID,FirstName,LastName,Street,City,State,Zip,PrimaryPhone,SecondaryPhone,Email,ClientNotes,Status,Username")] Client client)
        {
            if (ModelState.IsValid)
            {
                client.Username = User.Identity.Name;
                string responseString = Communication.PostRequest(apiUrl, "api/Clients", User.Identity.Name, JsonConvert.SerializeObject(client));

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

            string responseString = Communication.GetRequest(apiUrl, "api/Clients/" + id, User.Identity.Name);
            Client client = JsonConvert.DeserializeObject<Client>(responseString);

            return View(client);
        }

        // POST: Clients/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ClientID,FirstName,LastName,Street,City,State,Zip,PrimaryPhone,SecondaryPhone,Email,ClientNotes,Status,Username")] Client client)
        {
            if (ModelState.IsValid)
            {
                client.Username = User.Identity.Name;
                string responseString = Communication.PutRequest(apiUrl, "api/Clients/" + client.ClientID, User.Identity.Name, JsonConvert.SerializeObject(client));

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

            string responseString = Communication.GetRequest(apiUrl, "api/Clients/" + id, User.Identity.Name);
            Client client = JsonConvert.DeserializeObject<Client>(responseString);

            return View(client);
        }

        // POST: Clients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            string responseString = Communication.DeleteRequest(apiUrl, "api/Clients/" + id, User.Identity.Name);
            Client client = JsonConvert.DeserializeObject<Client>(responseString);

            return RedirectToAction("Index");
        }
    }
}

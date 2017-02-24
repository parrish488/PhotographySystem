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
using System.Net.Http;
using System.Net.Http.Headers;
using System.IO;
using Newtonsoft.Json;

namespace PhotgraphyMVC.Controllers
{
    [Authorize]
    public class BillingsController : Controller
    {
        private PhotographerContext db = new PhotographerContext();

        private const string apiUrl = "http://localhost:57669/";

        // GET: Billings
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page, int? billingYearSelection)
        {
            ViewBag.LastNameSortParm = string.IsNullOrEmpty(sortOrder) ? "last_name_desc" : "";
            ViewBag.FirstNameSortParm = sortOrder == "first_name" ? "first_name_desc" : "first_name";
            ViewBag.DateSortParam = sortOrder == "billing_date" ? "billing_date_desc" : "billing_date";
            ViewBag.TotalParm = sortOrder == "total" ? "total_desc" : "total";
            ViewBag.BillingYearSelection = billingYearSelection;

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            string responseString = Communication.GetRequest(apiUrl, "api/Billings/Years", User.Identity.Name);
            ViewBag.BillingYears = JsonConvert.DeserializeObject<List<int>>(responseString);

            responseString = Communication.GetRequest(apiUrl, "api/Billings", User.Identity.Name);
            var bills = JsonConvert.DeserializeObject<IEnumerable<Billing>>(responseString);

            if (!string.IsNullOrEmpty(searchString))
            {
                bills = bills.Where(b => b.Client.LastName.Contains(searchString)
                                       || b.Client.FirstName.Contains(searchString)
                                       || b.Total.ToString().Contains(searchString));
            }

            if (billingYearSelection != null && billingYearSelection != 0)
            {
                bills = bills.Where(e => e.BillingDate.Year == billingYearSelection);
            }
            else if (billingYearSelection == null)
            {
                bills = bills.Where(e => e.BillingDate.Year == DateTime.Now.Year);
                ViewBag.BillingYearSelection = DateTime.Now.Year;
            }

            switch (sortOrder)
            {
                case "total":
                    bills = bills.OrderBy(c => c.Total);
                    break;
                case "total_desc":
                    bills = bills.OrderByDescending(c => c.Total);
                    break;
                case "billing_date":
                    bills = bills.OrderBy(c => c.BillingDate);
                    break;
                case "billing_date_desc":
                    bills = bills.OrderByDescending(c => c.BillingDate);
                    break;
                case "first_name":
                    bills = bills.OrderBy(c => c.Client.FirstName);
                    break;
                case "first_name_desc":
                    bills = bills.OrderByDescending(c => c.Client.FirstName);
                    break;
                case "last_name_desc":
                    bills = bills.OrderByDescending(c => c.Client.LastName);
                    break;
                default:
                    bills = bills.OrderBy(c => c.Client.LastName);
                    break;
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(bills.ToPagedList(pageNumber, pageSize));
        }

        // GET: Billings/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            string responseString = Communication.GetRequest(apiUrl, "api/Billings/" + id, User.Identity.Name);
            Billing bill = JsonConvert.DeserializeObject<Billing>(responseString);

            return View(bill);
        }

        // GET: Billings/Create
        public ActionResult Create()
        {
            string responseString = Communication.GetRequest(apiUrl, "api/Clients", User.Identity.Name);
            var clients = JsonConvert.DeserializeObject<IEnumerable<Client>>(responseString);

            responseString = Communication.GetRequest(apiUrl, "api/TaxYears", User.Identity.Name);
            var taxYears = JsonConvert.DeserializeObject<IEnumerable<TaxYear>>(responseString);

            ViewBag.ClientID = new SelectList(clients, "ClientID", "FullName");
            ViewBag.TaxYearID = new SelectList(taxYears, "TaxYearID", "Year");

            return View();
        }

        // POST: Billings/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "BillingID,BillingDate,BillingType,Subtotal,SalesTax,Total,Username,ClientID,TaxYearID")] Billing billing)
        {
            if (ModelState.IsValid)
            {
                billing.Username = User.Identity.Name;

                string responseString = Communication.PostRequest(apiUrl, "api/Billings", User.Identity.Name, JsonConvert.SerializeObject(billing));

                return RedirectToAction("Index");
            }

            return View(billing);
        }

        // GET: Billings/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            string responseString = Communication.GetRequest(apiUrl, "api/Billings/" + id, User.Identity.Name);
            Billing bill = JsonConvert.DeserializeObject<Billing>(responseString);

            responseString = Communication.GetRequest(apiUrl, "api/Clients", User.Identity.Name);
            var clients = JsonConvert.DeserializeObject<IEnumerable<Client>>(responseString);

            responseString = Communication.GetRequest(apiUrl, "api/TaxYears", User.Identity.Name);
            var taxYears = JsonConvert.DeserializeObject<IEnumerable<TaxYear>>(responseString);

            bill.ClientIDs = clients.ToList();
            bill.TaxYearIDs = taxYears.ToList();

            return View(bill);
        }

        // POST: Billings/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "BillingID,BillingDate,BillingType,Subtotal,SalesTax,Total,Username,ClientID,TaxYearID")] Billing billing)
        {
            if (ModelState.IsValid)
            {
                billing.Username = User.Identity.Name;

                string responseString = Communication.PutRequest(apiUrl, "api/Billings/" + billing.BillingID, User.Identity.Name, JsonConvert.SerializeObject(billing));

                return RedirectToAction("Index");
            }

            return View(billing);
        }

        // GET: Billings/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            string responseString = Communication.GetRequest(apiUrl, "api/Billings/" + id, User.Identity.Name);
            Billing bill = JsonConvert.DeserializeObject<Billing>(responseString);

            return View(bill);
        }

        // POST: Billings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            string responseString = Communication.DeleteRequest(apiUrl, "api/Billings/" + id, User.Identity.Name);
            Billing bill = JsonConvert.DeserializeObject<Billing>(responseString);

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

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
using Newtonsoft.Json;

namespace PhotgraphyMVC.Controllers
{
    [Authorize]
    public class BillingsController : Controller
    {
        private PhotographerContext db = new PhotographerContext();

        const string apiUrl = @"http://localhost:57669/";

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

            //var billingYears = (from b in db.Billing
            //                    where b.Username == User.Identity.Name
            //                    select b.BillingDate.Year).Distinct();

            //ViewBag.BillingYears = billingYears.OrderByDescending(b => b).ToList();

            //var bills = from b in db.Billing where b.Username == User.Identity.Name
            //            select b;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(apiUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("username", User.Identity.Name);
                var response = client.GetAsync("api/Billings").Result;
                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadAsStringAsync().Result;
                    var bills = JsonConvert.DeserializeObject<IQueryable<Billing>>(response.Content.ReadAsStringAsync().Result);

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
                            bills = bills.OrderBy(c => c.Total).Include(b => b.Client).Include(b => b.TaxYear);
                            break;
                        case "total_desc":
                            bills = bills.OrderByDescending(c => c.Total).Include(b => b.Client).Include(b => b.TaxYear);
                            break;
                        case "mileage_date":
                            bills = bills.OrderBy(c => c.BillingDate).Include(m => m.Client).Include(m => m.TaxYear);
                            break;
                        case "mileage_date_desc":
                            bills = bills.OrderByDescending(c => c.BillingDate).Include(m => m.Client).Include(m => m.TaxYear);
                            break;
                        case "first_name":
                            bills = bills.OrderBy(c => c.Client.FirstName).Include(b => b.Client).Include(b => b.TaxYear);
                            break;
                        case "first_name_desc":
                            bills = bills.OrderByDescending(c => c.Client.FirstName).Include(b => b.Client).Include(b => b.TaxYear);
                            break;
                        case "last_name_desc":
                            bills = bills.OrderByDescending(c => c.Client.LastName).Include(b => b.Client).Include(b => b.TaxYear);
                            break;
                        default:
                            bills = bills.OrderBy(c => c.Client.LastName).Include(b => b.Client).Include(b => b.TaxYear);
                            break;
                    }

                    int pageSize = 10;
                    int pageNumber = (page ?? 1);
                    return View(bills.ToPagedList(pageNumber, pageSize));
                }
            }

            return HttpNotFound();
        }

        // GET: Billings/Details/5
        public ActionResult Details(int? id)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(apiUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var response = client.GetAsync("api/Billings/id").Result;
                if (response.IsSuccessStatusCode)
                {
                    var bill = JsonConvert.DeserializeObject<Billing>(response.Content.ReadAsStringAsync().Result);

                    return View(bill);
                }
            }

            return HttpNotFound();
        }

        // GET: Billings/Create
        public ActionResult Create()
        {
            var clients = from c in db.Clients
                          where c.Username == User.Identity.Name
                          orderby c.LastName ascending
                          select c;

            var taxYears = from t in db.TaxYears
                           where t.Username == User.Identity.Name
                           orderby t.Year descending
                           select t;

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
                TaxYear taxYear = db.TaxYears.Find(billing.TaxYearID);

                // Calculate sales tax for payment
                if (billing.BillingType == "Payment")
                {
                    decimal salesTax = billing.GetSalesTax(billing, taxYear);
                    billing.Subtotal = billing.Total - salesTax;
                    billing.SalesTax = salesTax;
                }

                billing.Username = User.Identity.Name;

                db.Billing.Add(billing);
                db.SaveChanges();

                if (billing.BillingType == "Payment")
                {
                    taxYear.TotalTax += billing.SalesTax;
                    taxYear.TotalGrossIncome += billing.Subtotal;
                }
                else if (billing.BillingType == "Expense")
                {
                    taxYear.TotalExpenses += billing.Total;
                }

                taxYear.TotalNetIncome = taxYear.TotalGrossIncome - taxYear.TotalExpenses;

                db.Entry(taxYear).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            //var clients = from c in db.Clients
            //              where c.Username == User.Identity.Name
            //              orderby c.LastName ascending
            //              select c;

            //var taxYears = from t in db.TaxYears
            //               where t.Username == User.Identity.Name
            //               orderby t.Year ascending
            //               select t;

            //ViewBag.ClientID = new SelectList(clients, "ClientID", "FullName", billing.ClientID);
            //ViewBag.TaxYearID = new SelectList(taxYears, "TaxYearID", "Year", billing.TaxYearID);
            return View(billing);
        }

        // GET: Billings/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Billing bill = db.Billing.Find(id);

            if (bill == null)
            {
                return HttpNotFound();
            }

            var clients = from c in db.Clients
                          where c.Username == User.Identity.Name
                          orderby c.LastName ascending
                          select c;

            var taxYears = from t in db.TaxYears
                           where t.Username == User.Identity.Name
                           orderby t.Year descending
                           select t;

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
                TaxYear taxYear = db.TaxYears.Find(billing.TaxYearID);

                // Calculate sales tax for payment
                if (billing.BillingType == "Payment")
                {
                    decimal salesTax = billing.GetSalesTax(billing, taxYear);
                    billing.Subtotal = billing.Total - salesTax;
                    billing.SalesTax = salesTax;
                }

                billing.Username = User.Identity.Name;

                db.Entry(billing).State = EntityState.Modified;
                db.SaveChanges();

                taxYear.TotalTax = 0;
                taxYear.TotalExpenses = 0;
                taxYear.TotalGrossIncome = 0;

                foreach (Billing bill in db.Billing)
                {
                    if (bill.TaxYearID == billing.TaxYearID)
                    {
                        if (bill.BillingType == "Payment")
                        {
                            taxYear.TotalTax += bill.SalesTax;
                            taxYear.TotalGrossIncome += bill.Subtotal;
                        }
                        else if (bill.BillingType == "Expense")
                        {
                            taxYear.TotalExpenses += bill.Total;
                        }
                    }
                }

                taxYear.TotalNetIncome = taxYear.TotalGrossIncome - taxYear.TotalExpenses;

                db.Entry(taxYear).State = EntityState.Modified;

                db.SaveChanges();
                return RedirectToAction("Index");
            }

            //var clients = from c in db.Clients
            //              where c.Username == User.Identity.Name
            //              orderby c.LastName ascending
            //              select c;

            //var taxYears = from t in db.TaxYears
            //               where t.Username == User.Identity.Name
            //               orderby t.Year ascending
            //               select t;

            //ViewBag.ClientID = new SelectList(clients, "ClientID", "FullName", billing.ClientID);
            //ViewBag.TaxYearID = new SelectList(taxYears, "TaxYearID", "Year", billing.TaxYearID);
            return View(billing);
        }

        // GET: Billings/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Billing bill = db.Billing.Find(id);

            if (bill == null)
            {
                return HttpNotFound();
            }

            return View(bill);
        }

        // POST: Billings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Billing billing = db.Billing.Find(id);
            db.Billing.Remove(billing);
            db.SaveChanges();

            TaxYear taxYear = db.TaxYears.Find(billing.TaxYearID);
            taxYear.TotalTax = 0;
            taxYear.TotalExpenses = 0;
            taxYear.TotalGrossIncome = 0;

            foreach (Billing bill in db.Billing)
            {
                if (bill.TaxYearID == billing.TaxYearID)
                {
                    if (bill.BillingType == "Payment")
                    {
                        taxYear.TotalTax += bill.SalesTax;
                        taxYear.TotalGrossIncome += bill.Subtotal;
                    }
                    else if (bill.BillingType == "Expense")
                    {
                        taxYear.TotalExpenses += bill.Total;
                    }
                }
            }

            taxYear.TotalNetIncome = taxYear.TotalGrossIncome - taxYear.TotalExpenses;

            db.Entry(taxYear).State = EntityState.Modified;

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

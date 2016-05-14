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
    public class BillingsController : Controller
    {
        private PhotographerContext db = new PhotographerContext();

        // GET: Billings
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.LastNameSortParm = String.IsNullOrEmpty(sortOrder) ? "last_name_desc" : "";
            ViewBag.FirstNameSortParm = sortOrder == "first_name" ? "first_name_desc" : "first_name";
            ViewBag.DateSortParam = sortOrder == "billing_date" ? "billing_date_desc" : "billing_date";
            ViewBag.TotalParm = sortOrder == "total" ? "total_desc" : "total";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var bills = from b in db.Billing
                             select b;
            if (!String.IsNullOrEmpty(searchString))
            {
                bills = bills.Where(b => b.Client.LastName.Contains(searchString)
                                       || b.Client.FirstName.Contains(searchString)
                                       //|| b.ClientEvent.EventType.Contains(searchString)
                                       || b.Total.ToString().Contains(searchString));
            }

            switch (sortOrder)
            {
                case "total":
                    bills = bills.OrderBy(c => c.Total).Include(b => b.Client)/*.Include(b => b.ClientEvent)*/.Include(b => b.TaxYear);
                    break;
                case "total_desc":
                    bills = bills.OrderByDescending(c => c.Total).Include(b => b.Client)/*.Include(b => b.ClientEvent)*/.Include(b => b.TaxYear);
                    break;
                case "mileage_date":
                    bills = bills.OrderBy(c => c.BillingDate).Include(m => m.Client)/*.Include(m => m.ClientEvent)*/.Include(m => m.TaxYear);
                    break;
                case "mileage_date_desc":
                    bills = bills.OrderByDescending(c => c.BillingDate).Include(m => m.Client)/*.Include(m => m.ClientEvent)*/.Include(m => m.TaxYear);
                    break;
                case "first_name":
                    bills = bills.OrderBy(c => c.Client.FirstName).Include(b => b.Client)/*.Include(b => b.ClientEvent)*/.Include(b => b.TaxYear);
                    break;
                case "first_name_desc":
                    bills = bills.OrderByDescending(c => c.Client.FirstName).Include(b => b.Client)/*.Include(b => b.ClientEvent)*/.Include(b => b.TaxYear);
                    break;
                case "last_name_desc":
                    bills = bills.OrderByDescending(c => c.Client.LastName).Include(b => b.Client)/*.Include(b => b.ClientEvent)*/.Include(b => b.TaxYear);
                    break;
                default:
                    bills = bills.OrderBy(c => c.Client.LastName).Include(b => b.Client)/*.Include(b => b.ClientEvent)*/.Include(b => b.TaxYear);
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
            Billing billing = db.Billing.Find(id);
            if (billing == null)
            {
                return HttpNotFound();
            }
            return View(billing);
        }

        // GET: Billings/Create
        public ActionResult Create()
        {
            ViewBag.ClientID = new SelectList(db.Clients, "ClientID", "FullName");
            //ViewBag.EventID = new SelectList(db.Events, "EventID", "EventLabel");
            ViewBag.TaxYearID = new SelectList(db.TaxYears, "TaxYearID", "Year");
            return View();
        }

        // POST: Billings/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "BillingID,BillingDate,Subtotal,SalesTax,Total,ClientID,TaxYearID")] Billing billing)
        {
            if (ModelState.IsValid)
            {
                // Calculate sales tax
                TaxYear taxYear = db.TaxYears.Find(billing.TaxYearID);
                decimal salesTax = billing.GetSalesTax(billing, taxYear);

                billing.Subtotal = billing.Total - salesTax;
                billing.SalesTax = salesTax;

                db.Billing.Add(billing);
                db.SaveChanges();

                taxYear.TotalTax += billing.SalesTax;
                taxYear.TotalNetIncome += billing.Subtotal;
                db.Entry(taxYear).State = EntityState.Modified;

                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ClientID = new SelectList(db.Clients, "ClientID", "FullName", billing.ClientID);
            //ViewBag.EventID = new SelectList(db.Events, "EventID", "EventLabel", billing.EventID);
            ViewBag.TaxYearID = new SelectList(db.TaxYears, "TaxYearID", "Year", billing.TaxYearID);
            return View(billing);
        }

        // GET: Billings/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Billing billing = db.Billing.Find(id);
            if (billing == null)
            {
                return HttpNotFound();
            }
            ViewBag.ClientID = new SelectList(db.Clients, "ClientID", "FullName", billing.ClientID);
            //ViewBag.EventID = new SelectList(db.Events, "EventID", "EventLabel", billing.EventID);
            ViewBag.TaxYearID = new SelectList(db.TaxYears, "TaxYearID", "Year", billing.TaxYearID);
            return View(billing);
        }

        // POST: Billings/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "BillingID,BillingDate,Subtotal,SalesTax,Total,ClientID,TaxYearID")] Billing billing)
        {
            if (ModelState.IsValid)
            {
                // Calculate sales tax
                TaxYear taxYear = db.TaxYears.Find(billing.TaxYearID);
                decimal salesTax = billing.GetSalesTax(billing, taxYear);

                billing.Subtotal = billing.Total - salesTax;
                billing.SalesTax = salesTax;

                db.Entry(billing).State = EntityState.Modified;
                db.SaveChanges();

                taxYear.TotalTax = 0;
                taxYear.TotalNetIncome = 0;

                foreach (Billing bill in db.Billing)
                {
                    if (bill.TaxYearID == billing.TaxYearID)
                    {
                        taxYear.TotalTax += bill.SalesTax;
                        taxYear.TotalNetIncome += bill.Subtotal;
                    }
                }
                db.Entry(taxYear).State = EntityState.Modified;

                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ClientID = new SelectList(db.Clients, "ClientID", "FullName", billing.ClientID);
            //ViewBag.EventID = new SelectList(db.Events, "EventID", "EventLabel", billing.EventID);
            ViewBag.TaxYearID = new SelectList(db.TaxYears, "TaxYearID", "Year", billing.TaxYearID);
            return View(billing);
        }

        // GET: Billings/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Billing billing = db.Billing.Find(id);
            if (billing == null)
            {
                return HttpNotFound();
            }
            return View(billing);
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
            taxYear.TotalNetIncome = 0;

            foreach (Billing bill in db.Billing)
            {
                if (bill.TaxYearID == billing.TaxYearID)
                {
                    taxYear.TotalTax += bill.SalesTax;
                    taxYear.TotalNetIncome += bill.Subtotal;
                }
            }
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

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
            ViewBag.LastNameSortParm = string.IsNullOrEmpty(sortOrder) ? "last_name_desc" : "";
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

            string user = Convert.ToString(Session["Username"]);

            var bills = from b in db.Billing where b.Username == user
                             select b;

            if (!string.IsNullOrEmpty(searchString))
            {
                bills = bills.Where(b => b.Client.LastName.Contains(searchString)
                                       || b.Client.FirstName.Contains(searchString)
                                       || b.Total.ToString().Contains(searchString));
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

        // GET: Billings/Details/5
        public ActionResult Details(int? id)
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

        // GET: Billings/Create
        public ActionResult Create()
        {
            string user = Session["Username"].ToString();

            var clients = from c in db.Clients
                        where c.Username == user
                        select c;

            var taxYears = from t in db.TaxYears
                          where t.Username == user
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
                // Calculate sales tax
                TaxYear taxYear = db.TaxYears.Find(billing.TaxYearID);
                decimal salesTax = billing.GetSalesTax(billing, taxYear);

                billing.Subtotal = billing.Total - salesTax;
                billing.SalesTax = salesTax;

                billing.Username = Session["Username"].ToString();

                db.Billing.Add(billing);
                db.SaveChanges();

                taxYear.TotalTax += billing.SalesTax;
                taxYear.TotalNetIncome += billing.Subtotal;
                db.Entry(taxYear).State = EntityState.Modified;

                db.SaveChanges();
                return RedirectToAction("Index");
            }

            string user = Session["Username"].ToString();

            var clients = from c in db.Clients
                          where c.Username == user
                          select c;

            var taxYears = from t in db.TaxYears
                           where t.Username == user
                           select t;

            ViewBag.ClientID = new SelectList(clients, "ClientID", "FullName", billing.ClientID);
            ViewBag.TaxYearID = new SelectList(taxYears, "TaxYearID", "Year", billing.TaxYearID);
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

            string user = Session["Username"].ToString();

            var clients = from c in db.Clients
                          where c.Username == user
                          select c;

            var taxYears = from t in db.TaxYears
                           where t.Username == user
                           select t;

            ViewBag.ClientID = new SelectList(clients, "ClientID", "FullName");
            ViewBag.TaxYearID = new SelectList(taxYears, "TaxYearID", "Year");
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
                // Calculate sales tax
                TaxYear taxYear = db.TaxYears.Find(billing.TaxYearID);
                decimal salesTax = billing.GetSalesTax(billing, taxYear);

                billing.Subtotal = billing.Total - salesTax;
                billing.SalesTax = salesTax;

                billing.Username = Session["Username"].ToString();

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

            string user = Session["Username"].ToString();

            var clients = from c in db.Clients
                          where c.Username == user
                          select c;

            var taxYears = from t in db.TaxYears
                           where t.Username == user
                           select t;

            ViewBag.ClientID = new SelectList(clients, "ClientID", "FullName", billing.ClientID);
            ViewBag.TaxYearID = new SelectList(taxYears, "TaxYearID", "Year", billing.TaxYearID);
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

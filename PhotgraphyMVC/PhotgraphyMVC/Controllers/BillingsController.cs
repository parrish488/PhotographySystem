using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PhotgraphyMVC.Models;

namespace PhotgraphyMVC.Controllers
{
    [Authorize]
    public class BillingsController : Controller
    {
        private PhotographerContext db = new PhotographerContext();

        // GET: Billings
        public ActionResult Index()
        {
            var billing = db.Billing.Include(b => b.Client).Include(b => b.ClientEvent).Include(b => b.TaxYear);
            return View(billing.ToList());
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
            ViewBag.EventID = new SelectList(db.Events, "EventID", "EventLabel");
            ViewBag.TaxYearID = new SelectList(db.TaxYears, "TaxYearID", "Year");
            return View();
        }

        // POST: Billings/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "BillingID,Date,Subtotal,SalesTax,Total,ClientID,EventID,TaxYearID")] Billing billing)
        {
            if (ModelState.IsValid)
            {
                // Calculate sales tax
                decimal subtotal = Decimal.Round(Decimal.Divide(billing.Total, 1.066m), 2);
                decimal salesTax = Decimal.Round(Decimal.Subtract(billing.Total, subtotal), 2);

                billing.Subtotal = subtotal;
                billing.SalesTax = salesTax;

                db.Billing.Add(billing);
                db.SaveChanges();

                TaxYear taxYear = db.TaxYears.Find(billing.TaxYearID);
                taxYear.TotalTax += billing.SalesTax;
                db.Entry(taxYear).State = EntityState.Modified;

                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ClientID = new SelectList(db.Clients, "ClientID", "FullName", billing.ClientID);
            ViewBag.EventID = new SelectList(db.Events, "EventID", "EventLabel", billing.EventID);
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
            ViewBag.EventID = new SelectList(db.Events, "EventID", "EventLabel", billing.EventID);
            ViewBag.TaxYearID = new SelectList(db.TaxYears, "TaxYearID", "Year", billing.TaxYearID);
            return View(billing);
        }

        // POST: Billings/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "BillingID,Date,Subtotal,SalesTax,Total,ClientID,EventID,TaxYearID")] Billing billing)
        {
            if (ModelState.IsValid)
            {
                // Calculate sales tax
                decimal subtotal = Decimal.Round(Decimal.Divide(billing.Total, 1.066m), 2);
                decimal salesTax = Decimal.Round(Decimal.Subtract(billing.Total, subtotal), 2);

                billing.Subtotal = subtotal;
                billing.SalesTax = salesTax;

                db.Entry(billing).State = EntityState.Modified;
                db.SaveChanges();

                TaxYear taxYear = db.TaxYears.Find(billing.TaxYearID);
                taxYear.TotalTax = 0;

                foreach (Billing bill in db.Billing)
                {
                    taxYear.TotalTax += bill.SalesTax;
                }
                db.Entry(taxYear).State = EntityState.Modified;

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ClientID = new SelectList(db.Clients, "ClientID", "FullName", billing.ClientID);
            ViewBag.EventID = new SelectList(db.Events, "EventID", "EventLabel", billing.EventID);
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

            foreach (Billing bill in db.Billing)
            {
                taxYear.TotalTax += bill.SalesTax;
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

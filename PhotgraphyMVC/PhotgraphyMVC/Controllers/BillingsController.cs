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
            var billing = db.Billing.Include(b => b.Client).Include(b => b.ClientEvent);
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
            return View();
        }

        // POST: Billings/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "BillingID,Date,Subtotal,SalesTax,Total,ClientID,EventID")] Billing billing)
        {
            if (ModelState.IsValid)
            {
                // Calculate sales tax
                decimal subtotal = billing.Total - (billing.Total * .1m);
                decimal salesTax = subtotal * .066m;

                while (subtotal + salesTax != billing.Total)
                {
                    subtotal += .01m;
                    salesTax = subtotal * .066m;
                    string taxString = string.Format("{0:C}", salesTax);
                    salesTax = Decimal.Parse(taxString.Substring(1).Trim(','));
                }

                billing.Subtotal = subtotal;
                billing.SalesTax = salesTax;

                db.Billing.Add(billing);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ClientID = new SelectList(db.Clients, "ClientID", "FullName", billing.ClientID);
            ViewBag.EventID = new SelectList(db.Events, "EventID", "EventLabel", billing.EventID);
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
            return View(billing);
        }

        // POST: Billings/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "BillingID,Date,Subtotal,SalesTax,Total,ClientID,EventID")] Billing billing)
        {
            if (ModelState.IsValid)
            {
                // Calculate sales tax
                decimal subtotal = billing.Total - (billing.Total * .1m);
                decimal salesTax = subtotal * .066m;

                while (subtotal + salesTax != billing.Total)
                {
                    subtotal += .01m;
                    salesTax = subtotal * .066m;
                    string taxString = string.Format("{0:C}", salesTax);
                    salesTax = Decimal.Parse(taxString.Substring(1).Trim(','));
                }

                billing.Subtotal = subtotal;
                billing.SalesTax = salesTax;

                db.Entry(billing).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ClientID = new SelectList(db.Clients, "ClientID", "FullName", billing.ClientID);
            ViewBag.EventID = new SelectList(db.Events, "EventID", "EventLabel", billing.EventID);
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

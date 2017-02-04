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
    public class TaxYearsController : Controller
    {
        private PhotographerContext db = new PhotographerContext();

        // GET: TaxYears
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.YearSortParm = String.IsNullOrEmpty(sortOrder) ? "year_desc" : "";
            ViewBag.TotalTaxSortParm = sortOrder == "total_tax" ? "total_tax_desc" : "total_tax";
            ViewBag.TotalIncomeSortParm = sortOrder == "total_income" ? "total_income_desc" : "total_income";
            ViewBag.TotalMilesSortParm = sortOrder == "total_miles" ? "total_miles_desc" : "total_miles";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var taxYear = from t in db.TaxYears where t.Username == User.Identity.Name
                          select t;

            if (!string.IsNullOrEmpty(searchString))
            {
                taxYear = taxYear.Where(t => t.Year.ToString().Contains(searchString));
            }

            switch (sortOrder)
            {
                case "total_miles":
                    taxYear = taxYear.OrderBy(c => c.TotalMiles);
                    break;
                case "total_miles_desc":
                    taxYear = taxYear.OrderByDescending(c => c.TotalMiles);
                    break;
                case "total_income":
                    taxYear = taxYear.OrderBy(c => c.TotalGrossIncome);
                    break;
                case "total_income_desc":
                    taxYear = taxYear.OrderByDescending(c => c.TotalGrossIncome);
                    break;
                case "total_tax":
                    taxYear = taxYear.OrderBy(c => c.TotalTax);
                    break;
                case "total_tax_desc":
                    taxYear = taxYear.OrderByDescending(c => c.TotalTax);
                    break;
                case "year_desc":
                    taxYear = taxYear.OrderBy(c => c.Year);
                    break;
                default:
                    taxYear = taxYear.OrderByDescending(c => c.Year);
                    break;
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(taxYear.ToPagedList(pageNumber, pageSize));
        }

        // GET: TaxYears/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TaxYear taxYear = db.TaxYears.Find(id);
            if (taxYear == null)
            {
                return HttpNotFound();
            }
            return View(taxYear);
        }

        // GET: TaxYears/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TaxYears/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "TaxYearID,Year,TaxRate,TotalTax,TotalMiles,TaxablePercent,TotalExpenses,TotalGrossIncome,TotalNetIncome,Username")] TaxYear taxYear)
        {
            if (ModelState.IsValid)
            {
                taxYear.Username = User.Identity.Name;

                db.TaxYears.Add(taxYear);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(taxYear);
        }

        // GET: TaxYears/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TaxYear taxYear = db.TaxYears.Find(id);
            if (taxYear == null)
            {
                return HttpNotFound();
            }
            return View(taxYear);
        }

        // POST: TaxYears/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "TaxYearID,Year,TaxRate,TotalTax,TotalMiles,TaxablePercent,TotalExpenses,TotalGrossIncome,TotalNetIncome,Username")] TaxYear taxYear)
        {
            if (ModelState.IsValid)
            {
                taxYear = RecalculateBilling(taxYear);
                taxYear.Username = User.Identity.Name;
                db.Entry(taxYear).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(taxYear);
        }

        // GET: TaxYears/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TaxYear taxYear = db.TaxYears.Find(id);
            if (taxYear == null)
            {
                return HttpNotFound();
            }
            return View(taxYear);
        }

        // POST: TaxYears/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TaxYear taxYear = db.TaxYears.Find(id);
            db.TaxYears.Remove(taxYear);
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

        private TaxYear RecalculateBilling(TaxYear taxYear)
        {
            taxYear.TotalTax = 0;
            taxYear.TotalExpenses = 0;
            taxYear.TotalGrossIncome = 0;
            taxYear.TotalMiles = 0;

            foreach (Billing billing in db.Billing)
            {
                if (billing.TaxYearID == taxYear.TaxYearID)
                {
                    if (billing.BillingType == "Payment")
                    {
                        billing.SalesTax = billing.GetSalesTax(billing, taxYear);
                        billing.Subtotal = billing.Total - billing.SalesTax;

                        taxYear.TotalTax += billing.SalesTax;
                        taxYear.TotalGrossIncome += billing.Total;
                    }
                    else if (billing.BillingType == "Expense")
                    {
                        taxYear.TotalExpenses += billing.Total;
                    }
                }
            }

            foreach (Mileage mileage in db.Mileage)
            {
                if (mileage.TaxYearID == taxYear.TaxYearID)
                {
                    taxYear.TotalMiles += mileage.MilesDriven;
                }
            }

            taxYear.TotalNetIncome = taxYear.TotalGrossIncome - taxYear.TotalTax - taxYear.TotalExpenses;

            return taxYear;
        }
    }
}

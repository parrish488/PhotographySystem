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
    public class TaxYearsController : Controller
    {
        private const string apiUrl = "http://localhost:57669/";

        // GET: TaxYears
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.YearSortParm = string.IsNullOrEmpty(sortOrder) ? "year_desc" : "";
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

            string responseString = Communication.GetRequest(apiUrl, "api/TaxYears", User.Identity.Name);
            var taxYears = JsonConvert.DeserializeObject<IEnumerable<TaxYear>>(responseString);

            if (!string.IsNullOrEmpty(searchString))
            {
                taxYears = taxYears.Where(t => t.Year.ToString().Contains(searchString));
            }

            switch (sortOrder)
            {
                case "total_miles":
                    taxYears = taxYears.OrderBy(c => c.TotalMiles);
                    break;
                case "total_miles_desc":
                    taxYears = taxYears.OrderByDescending(c => c.TotalMiles);
                    break;
                case "total_income":
                    taxYears = taxYears.OrderBy(c => c.TotalGrossIncome);
                    break;
                case "total_income_desc":
                    taxYears = taxYears.OrderByDescending(c => c.TotalGrossIncome);
                    break;
                case "total_tax":
                    taxYears = taxYears.OrderBy(c => c.TotalTax);
                    break;
                case "total_tax_desc":
                    taxYears = taxYears.OrderByDescending(c => c.TotalTax);
                    break;
                case "year_desc":
                    taxYears = taxYears.OrderBy(c => c.Year);
                    break;
                default:
                    taxYears = taxYears.OrderByDescending(c => c.Year);
                    break;
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(taxYears.ToPagedList(pageNumber, pageSize));
        }

        // GET: TaxYears/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            string responseString = Communication.GetRequest(apiUrl, "api/TaxYears/" + id, User.Identity.Name);
            TaxYear taxYear = JsonConvert.DeserializeObject<TaxYear>(responseString);

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
                string responseString = Communication.PostRequest(apiUrl, "api/TaxYears", User.Identity.Name, JsonConvert.SerializeObject(taxYear));

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

            string responseString = Communication.GetRequest(apiUrl, "api/TaxYears/" + id, User.Identity.Name);
            TaxYear taxYear = JsonConvert.DeserializeObject<TaxYear>(responseString);

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
                taxYear.Username = User.Identity.Name;
                string responseString = Communication.PutRequest(apiUrl, "api/TaxYears/" + taxYear.TaxYearID, User.Identity.Name, JsonConvert.SerializeObject(taxYear));

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
            
            string responseString = Communication.GetRequest(apiUrl, "api/TaxYears/" + id, User.Identity.Name);
            TaxYear taxYear = JsonConvert.DeserializeObject<TaxYear>(responseString);

            return View(taxYear);
        }

        // POST: TaxYears/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            string responseString = Communication.DeleteRequest(apiUrl, "api/TaxYears/" + id, User.Identity.Name);
            TaxYear taxYear = JsonConvert.DeserializeObject<TaxYear>(responseString);

            return RedirectToAction("Index");
        }

        public ActionResult StateTaxInformation()
        {
            return View();
        }
    }
}

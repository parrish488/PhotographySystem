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
    public class MileagesController : Controller
    {
        private PhotographerContext db = new PhotographerContext();

        // GET: Mileages
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.LastNameSortParam = String.IsNullOrEmpty(sortOrder) ? "last_name_desc" : "";
            ViewBag.FirstNameSortParam = sortOrder == "first_name" ? "first_name_desc" : "first_name";
            ViewBag.DateSortParam = sortOrder == "mileage_date" ? "mileage_date_desc" : "mileage_date";
            ViewBag.YearSortParam = sortOrder == "year" ? "year_desc" : "year";
            ViewBag.MileageSortParam = sortOrder == "mileage" ? "mileage_desc" : "mileage";
            ViewBag.SourceSortParam = sortOrder == "source" ? "source_desc" : "source";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var mileages = from m in db.Mileage where m.Username == User.Identity.Name
                           select m;
            if (!string.IsNullOrEmpty(searchString))
            {
                mileages = mileages.Where(m => m.Client.LastName.Contains(searchString)
                                       || m.Client.FirstName.Contains(searchString)
                                       || m.Source.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "source":
                    mileages = mileages.OrderBy(c => c.Source).Include(m => m.Client).Include(m => m.TaxYear);
                    break;
                case "source_desc":
                    mileages = mileages.OrderByDescending(c => c.Source).Include(m => m.Client).Include(m => m.TaxYear);
                    break;
                case "mileage_date":
                    mileages = mileages.OrderBy(c => c.MileageDate).Include(m => m.Client).Include(m => m.TaxYear);
                    break;
                case "mileage_date_desc":
                    mileages = mileages.OrderByDescending(c => c.MileageDate).Include(m => m.Client).Include(m => m.TaxYear);
                    break;
                case "mileage":
                    mileages = mileages.OrderBy(c => c.MilesDriven).Include(m => m.Client).Include(m => m.TaxYear);
                    break;
                case "mileage_desc":
                    mileages = mileages.OrderByDescending(c => c.MilesDriven).Include(m => m.Client).Include(m => m.TaxYear);
                    break;
                case "year":
                    mileages = mileages.OrderBy(c => c.TaxYear.Year).Include(m => m.Client).Include(m => m.TaxYear);
                    break;
                case "year_desc":
                    mileages = mileages.OrderByDescending(c => c.TaxYear.Year).Include(m => m.Client).Include(m => m.TaxYear);
                    break;
                case "first_name":
                    mileages = mileages.OrderBy(c => c.Client.FirstName).Include(m => m.Client).Include(m => m.TaxYear);
                    break;
                case "first_name_desc":
                    mileages = mileages.OrderByDescending(c => c.Client.FirstName).Include(m => m.Client).Include(m => m.TaxYear);
                    break;
                case "last_name_desc":
                    mileages = mileages.OrderByDescending(c => c.Client.LastName).Include(m => m.Client).Include(m => m.TaxYear);
                    break;
                default:
                    mileages = mileages.OrderBy(c => c.Client.LastName).Include(m => m.Client).Include(m => m.TaxYear);
                    break;
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(mileages.ToPagedList(pageNumber, pageSize));
        }

        // GET: Mileages/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Mileage mileage = db.Mileage.Find(id);
            if (mileage == null)
            {
                return HttpNotFound();
            }
            return View(mileage);
        }

        // GET: Mileages/Create
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

        // POST: Mileages/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MileageID,ClientID,TaxYearID,MileageDate,MilesDriven,Source,Username")] Mileage mileage)
        {
            if (ModelState.IsValid)
            {
                mileage.Username = User.Identity.Name;
                db.Mileage.Add(mileage);

                TaxYear taxYear = db.TaxYears.Find(mileage.TaxYearID);
                taxYear.TotalMiles += mileage.MilesDriven;

                taxYear.Username = User.Identity.Name;

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

            //ViewBag.ClientID = new SelectList(clients, "ClientID", "FullName", mileage.ClientID);
            //ViewBag.TaxYearID = new SelectList(taxYears, "TaxYearID", "Year", mileage.TaxYearID);

            return View(mileage);
        }

        // GET: Mileages/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Mileage mileage = db.Mileage.Find(id);
            if (mileage == null)
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

            mileage.ClientIDs = clients.ToList();
            mileage.TaxYearIDs = taxYears.ToList();

            return View(mileage);
        }

        // POST: Mileages/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MileageID,ClientID,EventID,TaxYearID,MileageDate,MilesDriven,Source,Username")] Mileage mileage)
        {
            if (ModelState.IsValid)
            {
                mileage.Username = User.Identity.Name;

                db.Entry(mileage).State = EntityState.Modified;
                //db.SaveChanges();

                TaxYear taxYear = db.TaxYears.Find(mileage.TaxYearID);
                taxYear.TotalMiles = 0;

                foreach (Mileage miles in db.Mileage.Where(m => m.Username == mileage.Username))
                {
                    if (miles.TaxYearID == mileage.TaxYearID)
                    {
                        taxYear.TotalMiles += miles.MilesDriven;
                    }
                }

                taxYear.Username = User.Identity.Name;

                db.Entry(taxYear).State = EntityState.Modified;
                db.SaveChanges();
                
                return RedirectToAction("Index");
            }

            //var clients = from c in db.Clients
            //              where c.Username == User.Identity.Name
            //              select c;

            //var taxYears = from t in db.TaxYears
            //               where t.Username == User.Identity.Name
            //               select t;

            //ViewBag.ClientID = new SelectList(clients, "ClientID", "FullName", mileage.ClientID);
            //ViewBag.TaxYearID = new SelectList(taxYears, "TaxYearID", "Year", mileage.TaxYearID);
            return View(mileage);
        }

        // GET: Mileages/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Mileage mileage = db.Mileage.Find(id);
            if (mileage == null)
            {
                return HttpNotFound();
            }
            return View(mileage);
        }

        // POST: Mileages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Mileage mileage = db.Mileage.Find(id);
            db.Mileage.Remove(mileage);
            db.SaveChanges();

            TaxYear taxYear = db.TaxYears.Find(mileage.TaxYearID);
            taxYear.TotalMiles = 0;

            foreach (Mileage miles in db.Mileage)
            {
                if (miles.TaxYearID == mileage.TaxYearID)
                {
                    taxYear.TotalMiles += miles.MilesDriven;
                }
            }

            taxYear.Username = User.Identity.Name;

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

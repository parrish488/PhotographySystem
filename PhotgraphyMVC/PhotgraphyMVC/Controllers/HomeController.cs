using PhotgraphyMVC.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PhotgraphyMVC.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private PhotographerContext db = new PhotographerContext();

        public ActionResult Index()
        {
            HomeData data = new HomeData();

            var events = from e in db.Events
                         where e.Username == User.Identity.Name
                         where e.EventDate >= DateTime.Now
                         orderby e.EventDate ascending
                         select e;

            data.UpcomingEvents = events.Take(5).ToList();

            var todoItems = from t in db.TodoList
                         where t.Username == User.Identity.Name
                         where t.IsCompleted == false
                         orderby t.DueDate ascending
                         select t;

            data.TodoListItems = todoItems.Take(5).ToList();

            List<TaxYear> years = db.TaxYears.Where(e => e.Username == User.Identity.Name).Where(t => t.Year == DateTime.Now.Year).ToList();
            TaxYear taxYear = new TaxYear();

            if (years.Count > 0)
            {
                taxYear = years[0];
            }

            data.TotalSalesTax = taxYear.TotalTax;
            data.TotalExpenses = taxYear.TotalExpenses;
            data.TotalGrossIncome = taxYear.TotalGrossIncome;
            data.TotalNetIncome = data.TotalGrossIncome - data.TotalSalesTax - data.TotalExpenses;
            data.MilesDriven = (int)taxYear.TotalMiles;

            VerifyActiveStatus(db, User.Identity.Name);

            return View(data);
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Contact Me";

            return View();
        }

        public ActionResult StateTaxInformation()
        {
            ViewBag.Message = "State Tax Information";

            return View();
        }

        public static void VerifyActiveStatus(PhotographerContext db, string user)
        {
            HashSet<int> activeClients = new HashSet<int>();

            var eventList = from e in db.Events
                         where e.Username == user
                         select e;

            foreach (Event clientEvent in eventList)
            {
                if (clientEvent.EventDate > DateTime.Now.AddMonths(-1))
                {
                    activeClients.Add(clientEvent.ClientID);
                }
            }

            var clientList = from c in db.Clients
                            where c.Username == user
                            select c;

            foreach (Client client in clientList)
            {
                if (activeClients.Contains(client.ClientID))
                {
                    client.Status = "Active";
                }
                else
                {
                    client.Status = "Inactive";
                }

                db.Entry(client).State = EntityState.Modified;
            }

            db.SaveChanges();
        }
    }
}
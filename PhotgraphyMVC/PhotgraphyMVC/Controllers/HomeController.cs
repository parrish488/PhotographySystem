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

            foreach (Event evnt in db.Events.Where(e => e.Username == User.Identity.Name))
            {
                if (evnt.EventDate >= DateTime.Now && evnt.EventDate < DateTime.Now.AddDays(30))
                {
                    data.UpcomingEvents.Add(evnt);
                }
            }

            foreach (TodoList todo in db.TodoList.Where(e => e.Username == User.Identity.Name))
            {
                //if (!todo.IsCompleted)
                //{
                    data.TodoListItems.Add(todo);
                //}
            }

            TaxYear taxYear = new TaxYear();

            foreach (TaxYear year in db.TaxYears.Where(e => e.Username == User.Identity.Name))
            {
                if (year.Year == DateTime.Now.Year)
                {
                    taxYear = year;
                }
            }

            data.TotalSalesTax = taxYear.TotalTax;
            data.TotalExpenses = taxYear.TotalExpenses;
            data.TotalGrossIncome = taxYear.TotalGrossIncome;
            data.TotalNetIncome = data.TotalGrossIncome - data.TotalExpenses;
            data.MilesDriven = (int)taxYear.TotalMiles;
            data.UpcomingEvents =  data.UpcomingEvents.OrderBy(x => x.EventDate).ToList();
            data.TodoListItems = data.TodoListItems.OrderBy(x => x.DueDate).ToList();

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

            var eventList = db.Events.Where(e => e.Username == user);

            foreach (Event clientEvent in eventList)
            {
                if (clientEvent.EventDate > DateTime.Now.AddMonths(-1))
                {
                    activeClients.Add(clientEvent.ClientID);
                }
            }

            var clientList = db.Clients;

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
using PhotgraphyMVC.Models;
using System;
using System.Collections.Generic;
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

            foreach (Billing bill in db.Billing)
            {
                data.TotalEarnings += bill.Subtotal;
            }

            foreach (Event evnt in db.Events)
            {
                if (evnt.EventDate >= DateTime.Now && evnt.EventDate < DateTime.Now.AddDays(30))
                {
                    data.UpcomingEvents.Add(evnt);
                }
            }

            foreach (TodoList todo in db.TodoList)
            {
                if (!todo.IsCompleted)
                {
                    data.TodoListItems.Add(todo);
                }
            }

            TaxYear taxYear = new TaxYear();

            foreach (TaxYear year in db.TaxYears)
            {
                if (year.Year == DateTime.Now.Year)
                {
                    taxYear = year;
                }
            }

            data.TotalSalesTax = taxYear.TotalTax;
            data.MilesDriven = (int)taxYear.TotalMiles;
            data.UpcomingEvents =  data.UpcomingEvents.OrderBy(x => x.EventDate).ToList();
            data.TodoListItems = data.TodoListItems.OrderBy(x => x.DueDate).ToList();

            return View(data);
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Contact Me";

            return View();
        }
    }
}
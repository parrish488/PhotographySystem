using PhotgraphyMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PhotgraphyMVC.Controllers
{
    public class HomeController : Controller
    {
        private PhotographerContext db = new PhotographerContext();

        public ActionResult Index()
        {
            HomeData data = new HomeData();

            foreach (Billing bill in db.Billing)
            {
                data.TotalSalesTax += bill.SalesTax;
                data.TotalEarnings += bill.Subtotal;
            }

            foreach (Event evnt in db.Events)
            {
                if (evnt.EventDate >= DateTime.Now && evnt.EventDate < DateTime.Now.AddDays(30))
                {
                    data.UpcomingEvents.Add(evnt);
                }
            }

            data.UpcomingEvents =  data.UpcomingEvents.OrderBy(x => x.EventDate).ToList();

            return View(data);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Contact Me";

            return View();
        }
    }
}
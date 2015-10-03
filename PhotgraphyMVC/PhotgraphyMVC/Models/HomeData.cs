using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PhotgraphyMVC.Models
{
    public class HomeData
    {
        public decimal TotalSalesTax { get; set; }
        public decimal TotalEarnings { get; set; }

        public List<Event> UpcomingEvents { get; set; }
        public List<TodoList> TodoListItems { get; set; }

        public HomeData()
        {
            TotalSalesTax = 0;
            TotalEarnings = 0;
            UpcomingEvents = new List<Event>();
            TodoListItems = new List<TodoList>();
        }
    }
}
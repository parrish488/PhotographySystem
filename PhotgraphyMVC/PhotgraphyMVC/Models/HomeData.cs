using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PhotgraphyMVC.Models
{
    public class HomeData
    {
        public decimal TotalSalesTax { get; set; }
        public decimal TotalGrossIncome { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal TotalNetIncome { get; set; }
        public int MilesDriven { get; set; }

        public List<Event> UpcomingEvents { get; set; }
        public List<TodoList> TodoListItems { get; set; }

        public HomeData()
        {
            TotalSalesTax = 0;
            TotalGrossIncome = 0;
            TotalExpenses = 0;
            TotalNetIncome = 0;
            MilesDriven = 0;
            UpcomingEvents = new List<Event>();
            TodoListItems = new List<TodoList>();
        }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PhotgraphyMVC.Models
{
    public class Billing
    {
        public int BillingID { get; set; }
        
        public decimal Subtotal { get; set; }
        [Display(Name = "Sales Tax")]
        public decimal SalesTax { get; set; }
        public decimal Total { get; set; }

        [ForeignKey("Client")]
        public int ClientID { get; set; }
        public virtual Client Client { get; set; }

        [ForeignKey("ClientEvent")]
        public int EventID { get; set; }
        public virtual Event ClientEvent { get; set; }
    }
}
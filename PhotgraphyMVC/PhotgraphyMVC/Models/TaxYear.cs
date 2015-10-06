using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PhotgraphyMVC.Models
{
    public class TaxYear
    {
        public TaxYear()
        {
            TaxRate = 0;
            TotalTax = 0.0m;
            TotalMiles = 0;
        }

        public int TaxYearID { get; set; }

        public int Year { get; set; }

        [Display(Name = "Tax Rate")]
        public float TaxRate { get; set; }

        [Display(Name = "Total Tax")]
        public decimal TotalTax { get; set; }

        [Display(Name = "Total Miles")]
        public double TotalMiles { get; set; }

        [Display(Name = "Taxable Percent")]
        public decimal TaxablePercent { get; set; }
    }
}
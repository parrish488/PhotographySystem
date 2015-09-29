﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PhotgraphyMVC.Models
{
    public class Mileage
    {
        public int MileageID { get; set; }
        [Display(Name = "Mileage Driven")]
        public float MilesDriven { get; set; }
        public string Source { get; set; }

        [ForeignKey("Client")]
        public int ClientID { get; set; }
        public virtual Client Client { get; set; }

        [ForeignKey("ClientEvent")]
        public int EventID { get; set; }
        public virtual Event ClientEvent { get; set; }
    }
}
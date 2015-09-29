﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PhotgraphyMVC.Models
{
    public class Event
    {
        public int EventID { get; set; }

        [Display(Name = "Event Date")]
        public DateTime EventDate { get; set; }
        [Display(Name = "Event Type")]
        public string EventType { get; set; }

        public string EventLabel
        {
            get
            {
                return string.Format("{0}, {1} - {2}", Client.FullName, EventType, EventDate.ToShortDateString());
            }
        }

        [ForeignKey("Client")]
        public int ClientID { get; set; }        
        public virtual Client Client { get; set; }
    }
}
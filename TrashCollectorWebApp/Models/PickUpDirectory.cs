using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrashCollectorWebApp.Models
{
    public class PickUpDirectory
    {
        [Key]
        public int ID { get; set; }
        public string DayOfWeek { get; set; }
        public bool SpecialPickUp { get; set; }
        public string SpecialDate { get; set; }
        public bool PickUpCompleted { get; set; }
        [ForeignKey("Customer")]
        public int CustomerID { get; set; }
        public Customer Customer { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
    }
}
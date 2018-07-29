using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrashCollectorWebApp.Models
{
    public class Transaction
    {
        [Key]
        public int ID { get; set; }
        public double Amount { get; set; }
        public bool TransactionCompleted { get; set; }
        [ForeignKey("PickUp")]
        public int PickUpID { get; set; }
        public PickUpDirectory PickUp { get; set; }
    }
}
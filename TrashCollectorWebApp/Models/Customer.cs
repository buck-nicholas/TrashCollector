using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrashCollectorWebApp.Models
{
    public class Customer
    {
        
        [Key]
        public int ID { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string AddressLineOne { get; set; }
        public string AddressLineTwo { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string USState { get; set; }
        [Required]
        public int ZipCode { get; set; }
        [ForeignKey("ApplicationUser")]
        public string UserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
    }
}
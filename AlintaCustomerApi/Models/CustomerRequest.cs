using System;
using System.ComponentModel.DataAnnotations;

namespace AlintaCustomerApi.Models
{
    public class CustomerRequest
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set;}
    }
}

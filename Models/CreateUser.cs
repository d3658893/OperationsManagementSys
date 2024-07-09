using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OperationsManagementSystem.Models
{
    public class CreateUser
    {
        [Required(ErrorMessage = "Name is Required")]
        public string Username { get; set; }
        [Required(ErrorMessage = "Name is Required")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Name is Required")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Name is Required")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Name is Required")]
        public string TeamLeadEmail { get; set; }
        [Required(ErrorMessage = "Name is Required")]
        public string Team { get; set; }
    }
}
using System;
using System.ComponentModel.DataAnnotations;

namespace ActivityEvent.Models
{
    public class LoginUser
    {
        [Required]
        [Display(Name = "Email")]
        public string LoginEmail { get; set; }

        [Required]
        [Display(Name = "Password")]
        public string LoginPassword { get; set; }
    }
}
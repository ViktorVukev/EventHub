using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required(ErrorMessage = "Това поле е задължително.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Това поле е задължително.")]
        public string LastName { get; set; }
    }
}

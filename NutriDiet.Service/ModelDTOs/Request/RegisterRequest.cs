using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NutriDiet.Service.ModelDTOs.Request
{
    public class RegisterRequest
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Password is required")]
        [StringLength(20, MinimumLength = 6,
            ErrorMessage = "Password must be between 6 and 20 characters")]
        public string Password { get; set; } = null!;
    }
}

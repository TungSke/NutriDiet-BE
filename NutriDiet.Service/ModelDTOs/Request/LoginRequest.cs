using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NutriDiet.Service.ModelDTOs.Request
{
    public class LoginRequest
    {
        [EmailAddress]
        public string Email { get; set; } = null!;
        
        public string Password { get; set; } = null!;

        public string? fcmToken { get; set; }
    }
}

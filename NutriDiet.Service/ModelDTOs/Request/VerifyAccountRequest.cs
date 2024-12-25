using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NutriDiet.Service.ModelDTOs.Request
{
    public class VerifyAccountRequest
    {
        public string Email { get; set; }
        public string OTP { get; set; }
    }
}

using NutriDiet.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NutriDiet.Service.ModelDTOs.Request
{
    public class UpdateUserRequest
    {
        public string FullName { get; set; } = null!;

        public int? Age { get; set; }

        public Gender Gender { get; set; }

        public string? Location { get; set; }
    }
}

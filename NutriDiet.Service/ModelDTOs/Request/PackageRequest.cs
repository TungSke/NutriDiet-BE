using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NutriDiet.Service.ModelDTOs.Request
{
    public class PackageRequest
    {
        public string PackageName { get; set; } = null!;
        public double Price { get; set; }
        public int Duration { get; set; }
        public string? Description { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NutriDiet.Service.ModelDTOs.Request
{
    public class DiseaseRequest
    {
        public string DiseaseName { get; set; } = null!;

        public string? Description { get; set; }
    }
}

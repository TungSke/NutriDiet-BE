using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NutriDiet.Service.ModelDTOs.Response
{
    public class CuisineTypeResponse
    {
        public int CuisineId { get; set; }

        public string CuisineName { get; set; } = null!;
    }
}

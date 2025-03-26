using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NutriDiet.Service.ModelDTOs.Response
{
    public class DiseaseResponse
    {
        public int DiseaseId { get; set; }
        public string DiseaseName { get; set; } = null!;

        public string? Description { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
        public List<IngredientResponse> Ingredients { get; set; } = new List<IngredientResponse>();
    }
}

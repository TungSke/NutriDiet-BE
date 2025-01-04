using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NutriDiet.Service.ModelDTOs.Request
{
    public class FoodRequest
    {
        [Required]
        public string FoodName { get; set; } = null!;
        [Required]
        public string FoodType { get; set; }

        public IFormFile? FoodImageUrl { get; set; }

        public string? Description { get; set; }
        [Required]
        public string ServingSize { get; set; }

        public ICollection<FoodDetailRequest> foodDetailRequests { get; set; } = new List<FoodDetailRequest>();
    }
}

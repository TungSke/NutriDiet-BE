using Microsoft.AspNetCore.Http;
using NutriDiet.Common.Enums;

namespace NutriDiet.Service.ModelDTOs.Request
{
    public class UpdateUserRequest
    {
        public string FullName { get; set; } = null!;

        public int Age { get; set; }

        public Gender Gender { get; set; }

        public string Location { get; set; }

        public IFormFile? Avatar { get; set; }
    }
}

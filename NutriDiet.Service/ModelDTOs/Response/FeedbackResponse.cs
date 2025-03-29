using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NutriDiet.Service.ModelDTOs.Response
{
    public class FeedbackResponse
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public int UserId { get; set; }
        public DateTime? RecommendedAt { get; set; }
        public string? Response { get; set; }
        public string Status { get; set; }
        public string? RejectionReason { get; set; }
        public string? Feedback { get; set; }
    }
}

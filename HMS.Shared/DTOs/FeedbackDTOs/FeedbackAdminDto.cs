using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Shared.DTOs.FeedbackDTOs
{
    public class FeedbackAdminDto
    {
        public int Id { get; set; }
        public string UserEmail { get; set; } = null!;
        public string Content { get; set; } = null!;
        public string Status { get; set; } = null!;
        public string? AiResult { get; set; }
        public string? AiReason { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

using HMS.Core.Entities.SecurityModul;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Core.Entities.FeedbackModule
{
    public class Feedback : BaseEntity<int>
    {
        public string UserId { get; set; } = null!;
        public HotelUser Guest { get; set; } = null!;
        public string Content { get; set; } = null!;
        public FeedbackStatus Status { get; set; }

        // AI Moderation
        public string? AiResult { get; set; }       // Approved / Rejected
        public string? AiReason { get; set; }       // Offensive language, etc.

    }
}

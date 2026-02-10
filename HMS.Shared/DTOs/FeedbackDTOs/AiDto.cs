using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Shared.DTOs.FeedbackDTOs
{
    public class AiDto
    {
        public bool IsApproved { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}

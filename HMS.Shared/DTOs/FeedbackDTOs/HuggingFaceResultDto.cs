using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Shared.DTOs.FeedbackDTOs
{
    public class HuggingFaceResultDto
    {
        public string Label { get; set; } = default!;
        public float Score { get; set; }
    }
}

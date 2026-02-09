using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Shared.DTOs.FeedbackDTOs
{
    public class OpenAiModerationRequest
    {
        public string Model { get; set; } = null!;
        public string Input { get; set; } = null!;
    }
}

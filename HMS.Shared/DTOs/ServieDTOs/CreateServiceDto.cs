using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Shared.DTOs.ServieDTOs
{
    public class CreateServiceDto
    {
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        
    }
}

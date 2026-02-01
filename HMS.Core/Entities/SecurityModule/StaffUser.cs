using HMS.Core.Entities.SecurityModul;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Core.Entities.SecurityModule
{
    public class StaffUser : HotelUser
    {
        public StaffSpecialities specialities { get; set; }
    }
}

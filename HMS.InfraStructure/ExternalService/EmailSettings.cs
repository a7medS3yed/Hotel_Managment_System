using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.InfraStructure.ExternalService
{
    public class EmailSettings
    {
        public string From { get; set; } = null!;
        public string SmtpServer { get; set; } = null!;
        public int Port { get; set; }
        public string DisplayName { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}

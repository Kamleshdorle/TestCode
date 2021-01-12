using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterDataService.Models
{
    public class AuditLog
    {
        public string application { get; set; }
        public string level { get; set; }
        public string message { get; set; }
        public string logger { get; set; }
        public string callsite { get; set; }
        public string exception { get; set; }

        public string logname { get; set; }
        public string description { get; set; }
        public string status { get; set; }
        public string systemmessage { get; set; }
        public string typeofevent { get; set; }
        public string userid { get; set; }
        public string changeddetails { get; set; }
    }
}

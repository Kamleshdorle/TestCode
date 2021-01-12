using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterDataService.Models
{
    public class Trailer
    {
        //public int? id { get; set; }
        public string trailernumber { get; set; }
        public string numberplate { get; set; }
        //public string amount { get; set; }
        public string department { get; set; }
    }

    public class TrailerSQl
    {
        public string trailernumber { get; set; }
        public string numberplate { get; set; }
        public string department { get; set; }
    }
}

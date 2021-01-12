using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterDataService.Models
{
    public class Company
    {
        //public int id { get; set; }
        public string name { get; set; }
        public string address { get; set; }
        public string vatnumber { get; set; }
        public string vendorgroup { get; set; }
    }
}

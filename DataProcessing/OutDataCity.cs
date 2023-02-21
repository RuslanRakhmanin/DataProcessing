using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataProcessing
{
    public class OutDataCity
    {
        public string city { get;  }
        public List<OutDataService> services { get; }
        public decimal total { get; }

        public OutDataCity(string city, List<OutDataService> services, decimal total)
        {
            this.city = city;
            this.services = services;
            this.total = total;
        }
    }
}

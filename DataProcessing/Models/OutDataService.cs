using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataProcessing.Models
{
    public class OutDataService
    {
        public string name { get; }
        public List<OutDataPayer> payers { get; }
        public decimal total { get; }

        public OutDataService(string name, List<OutDataPayer> payers, decimal total)
        {
            this.name = name;
            this.payers = payers;
            this.total = total;
        }

    }
}

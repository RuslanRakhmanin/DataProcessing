using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataProcessing.Models
{
    public class OutDataPayer
    {
        public string name { get; }
        public decimal payment { get; }
        public DateOnly date { get; }
        public long account_number { get; }
        public OutDataPayer(string name, decimal payment, DateOnly date, long account_number)
        {
            this.name = name;
            this.payment = payment;
            this.date = date;
            this.account_number = account_number;
        }
    }
}

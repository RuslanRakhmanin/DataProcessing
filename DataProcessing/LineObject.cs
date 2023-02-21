using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataProcessing
{
    public class LineObject
    {
        public string firstName { get; }
        public string lastName { get; }
        public string payerName{ get; }
        public string address { get; }
        public string city { get; }
        public decimal payment { get; }
        public DateOnly date { get; }
        public long accountNumber { get; }
        public string service { get; }

        public LineObject(string firstName, string lastName, string address, string city, decimal payment, DateOnly date, long accountNumber, string service)
        {
            this.firstName = firstName;
            this.lastName = lastName;
            this.address = address;
            this.city = city;
            this.payment = payment;
            this.date = date;
            this.accountNumber = accountNumber;
            this.service = service;
            this.payerName = firstName + " " + lastName;
        }

        public override string ToString()
        {
            return firstName + "|" + lastName + "|" + address + "|" + payment + "|" + date + "|" + accountNumber + "|" + service;
        }
    }
}

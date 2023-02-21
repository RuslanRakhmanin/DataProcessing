using NReco.Csv;
using System;
using System.ComponentModel;
using System.Net;
using System.Text;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DataProcessing // Note: actual namespace depends on the project name.
{
    internal class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine("Hello World!");
            List<LineObject> dataLines = new List<LineObject>();

            try
            {
                bool firstLine = true;
                var inputText = System.IO.File.ReadLines(@"D:\Work\dotNET\DataProcessing\raw_data.csv");

                foreach (var line in inputText)
                {
                    if (firstLine)
                    {
                        firstLine = false;
                        continue;
                    }
                    dataLines.Add(LineParser.ParseLine(line));
                    //Console.WriteLine(firstName + "|" + lastName + "|" + address + "|" + payment + "|" + date + "|" + accountNumber + "|" + service);
                    //Console.WriteLine(dataLines.Last().ToString() );
                }

            }
            catch (IOException e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }

            dataLines.Where(x => x.payment > 500).ToList().ForEach(x => Console.WriteLine(x));
            Console.WriteLine(JsonSerializer.Serialize(dataLines.ToArray()));
            
            HashSet<string> cities= new HashSet<string>();
            HashSet<string> services = new HashSet<string>();

            dataLines.ForEach(x => cities.Add(x.city));
            dataLines.ForEach(x => services.Add(x.service));

            foreach (var cityName in cities)
            {
                List<OutDataService> listOfServices = new List<OutDataService>();
                foreach (var serviceName in services)
                {
                    List<OutDataPayer> listOfPayers = new List<OutDataPayer>();
                    decimal total = 0;
                    dataLines.Where(x => x.city == cityName && x.service == serviceName)
                        .ToList().ForEach(x => { 
                            listOfPayers.Add(new OutDataPayer(x.payerName, x.payment, x.date, x.accountNumber));
                            total += x.payment;
                        } );
                    listOfServices.Add(new OutDataService(serviceName, listOfPayers, total));
                }
            }
        }
    }
}
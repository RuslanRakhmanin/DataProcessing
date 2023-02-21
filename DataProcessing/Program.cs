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
        static (int, int) ProcessFile(string inputFileName, string outFileName)
        {

            int linesParsed = 0;
            int linesError = 0;
            List<LineObject> dataLines = new List<LineObject>();

            try
            {
                bool firstLine = true;
                var inputText = System.IO.File.ReadLines(inputFileName);

                foreach (var line in inputText)
                {
                    if (firstLine)
                    {
                        firstLine = false;
                        continue;
                    }
                    dataLines.Add(LineParser.ParseLine(line));
                    linesParsed++;
                }

            }
            catch (IOException e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }

            HashSet<string> cities = new HashSet<string>();
            HashSet<string> services = new HashSet<string>();

            dataLines.ForEach(x => cities.Add(x.city));
            dataLines.ForEach(x => services.Add(x.service));

            List<OutDataCity> listOfCities = new List<OutDataCity>();

            foreach (var cityName in cities)
            {
                List<OutDataService> listOfServices = new List<OutDataService>();
                decimal totalCityLevel = 0;

                foreach (var serviceName in services)
                {
                    List<OutDataPayer> listOfPayers = new List<OutDataPayer>();
                    decimal totalServiceLevel = 0;
                    dataLines.Where(x => x.city == cityName && x.service == serviceName)
                        .ToList().ForEach(x => {
                            listOfPayers.Add(new OutDataPayer(x.payerName, x.payment, x.date, x.accountNumber));
                            totalServiceLevel += x.payment;
                        });
                    listOfServices.Add(new OutDataService(serviceName, listOfPayers, totalServiceLevel));
                    totalCityLevel += totalServiceLevel;
                }
                listOfCities.Add(new OutDataCity(cityName, listOfServices, totalCityLevel));
            }

            var options = new JsonSerializerOptions { WriteIndented = true };

            //Console.WriteLine(JsonSerializer.Serialize(listOfCities.ToArray(), options));
            using FileStream createStream = File.Create(outFileName);
            JsonSerializer.Serialize(createStream, listOfCities.ToArray(), options);
            createStream.Dispose();

            return (linesParsed, linesError);
        }

        static void Main(string[] args)
        {

            string outFolder;
            string fileNameIn, fileNameOut;
            int filesParsedTotal = 0;
            int linesParsed, linesParsedTotal = 0;
            int linesError, linesErrorTotal = 0;
            string FolderInput = "D:\\Work\\dotNET\\DataProcessing\\folder_a";
            string FolderOutput = "D:\\Work\\dotNET\\DataProcessing\\folder_b";

            fileNameIn = Path.Combine(FolderInput, "raw_data.csv");

            outFolder = Path.Combine(FolderOutput, DateTime.Now.ToString("dd-MM-yyyy"));
            if ( !Directory.Exists(outFolder))
            {
                Directory.CreateDirectory(outFolder);
            }
            fileNameOut = Path.Combine(outFolder, string.Format("output{0}.json", filesParsedTotal + 1));

            (linesParsed, linesError) = ProcessFile(fileNameIn, fileNameOut);

            linesParsedTotal += linesParsed;
            linesErrorTotal += linesError;
            filesParsedTotal++;

            Console.WriteLine("parsed_files: {0}", filesParsedTotal);
            Console.WriteLine("parsed_lines: {0}", linesParsedTotal);
            Console.WriteLine("found_errors: {0}", linesErrorTotal);
        }
    }
}
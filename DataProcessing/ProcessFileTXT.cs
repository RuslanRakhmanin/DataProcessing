using DataProcessing.Interfaces;
using DataProcessing.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DataProcessing
{
    internal class ProcessFileTXT: IProcessFile
    {
        public ProcessFileTXT() { }

        public bool IsFileReady(string filename)
        {
            try
            {
                using (FileStream inputStream = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.None))
                    return inputStream.Length > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public virtual (IEnumerable<string>, bool) GetLinesFromFile(string inputFileName)
        {
            while (!IsFileReady(inputFileName) && File.Exists(inputFileName))
            {

            }

            if  (!File.Exists(inputFileName))
            {
                return (Enumerable.Empty<string>(), true);
            }

            try
            {
                IEnumerable<string> inputText = System.IO.File.ReadLines(inputFileName);
                return (inputText, false);
            }
            catch (IOException e)
            {
                return (Enumerable.Empty<string>(), true);
            }

        }
        public void ProcessFile(string inputFileName, string outFileName, FilesProcessing FileProcessingObject, CancellationToken cancellationToken)
        {
            int linesParsed = 0;
            int linesError = 0;
            LineObject? lineObject;
            List<LineObject> dataLines = new List<LineObject>();
            IEnumerable<string> inputText;
            bool error;

            (inputText, error) = GetLinesFromFile(inputFileName);

            if (error)
            {
                FileProcessingObject.OnFileProcessingEnd(0, 1, inputFileName);
                return;
            }

            foreach (var line in inputText)
            {
                cancellationToken.ThrowIfCancellationRequested();
                (lineObject, error) = LineParser.ParseLine(line);
                if (error)
                {
                    linesError++;
                } 
                else
                {
                    dataLines.Add(lineObject);
                    linesParsed++;
                }
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

                    cancellationToken.ThrowIfCancellationRequested();

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

            cancellationToken.ThrowIfCancellationRequested();

            FileProcessingObject.OnFileProcessingEnd(linesParsed, linesError, inputFileName);

        }
    }
}

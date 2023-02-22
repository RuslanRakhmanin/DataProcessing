using DataProcessing.Models;
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

            string FolderInput = "D:\\Work\\dotNET\\DataProcessing\\folder_a";
            string FolderOutput = "D:\\Work\\dotNET\\DataProcessing\\folder_b";

            LinesCounter counter = new LinesCounter();
            FilesCounter filesCounter = new FilesCounter(FolderOutput);

            ProcessFileTXT txtFileProcessor = new ProcessFileTXT();
            FilesProcessing txtProcessing = new FilesProcessing(FolderInput, "*.txt",FolderOutput, txtFileProcessor, counter, filesCounter);
            Task.Run(() => txtProcessing.StartProcessingFileChanges());

            ProcessFileCSV csvFileProcessor = new ProcessFileCSV();
            FilesProcessing csvProcessing = new FilesProcessing(FolderInput, "*.csv", FolderOutput, csvFileProcessor, counter, filesCounter);
            Task.Run(() => csvProcessing.StartProcessingFileChanges());


            Console.WriteLine("Press Enter to exit");
            Console.ReadLine();

            txtProcessing.Dispose();
            csvProcessing.Dispose();

            Console.WriteLine(counter);

            //string outFolder;
            //string fileNameIn, fileNameOut;
            //int filesParsedTotal = 0;
            //int linesParsed, linesParsedTotal = 0;
            //int linesError, linesErrorTotal = 0;
            //List<string> filesInvalid = new List<string>();


            //fileNameIn = Path.Combine(FolderInput, "raw_data.csv");

            //outFolder = Path.Combine(FolderOutput, DateTime.Now.ToString("MM-dd-yyyy"));
            //if ( !Directory.Exists(outFolder))
            //{
            //    Directory.CreateDirectory(outFolder);
            //}
            //fileNameOut = Path.Combine(outFolder, string.Format("output{0}.json", filesParsedTotal + 1));

            //(linesParsed, linesError, _) = ProcessFile(fileNameIn, fileNameOut);

            //linesParsedTotal += linesParsed;
            //linesErrorTotal += linesError;
            //filesParsedTotal++;
            //if (linesError > 0)
            //{
            //    filesInvalid.Add(fileNameIn);
            //}

            //Console.WriteLine("parsed_files: {0}", filesParsedTotal);
            //Console.WriteLine("parsed_lines: {0}", linesParsedTotal);
            //Console.WriteLine("found_errors: {0}", linesErrorTotal);
            //Console.WriteLine(JsonSerializer.Serialize(filesInvalid));
        }
    }
}
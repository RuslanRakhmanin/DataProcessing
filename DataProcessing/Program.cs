using DataProcessing.Models;
using NReco.Csv;
using System;
using System.ComponentModel;
using System.Net;
using System.Text;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Configuration;
using System.Collections.Specialized;

namespace DataProcessing // Note: actual namespace depends on the project name.
{
    internal class Program
    {
        static void Main(string[] args)
        {

            string FolderInput;
            string FolderOutput;
            string FolderMeta;

            if (ConfigurationManager.AppSettings.Get("folder_a") == null)
            {
                Console.WriteLine("Error. Folder A is not found in the configuration file.");
                return;
            }
            if (ConfigurationManager.AppSettings.Get("folder_b") == null)
            {
                Console.WriteLine("Error. Folder B is not found in the configuration file.");
                return;
            }
            if (ConfigurationManager.AppSettings.Get("folder_c") == null)
            {
                Console.WriteLine("Error. Folder C is not found in the configuration file.");
                return;
            }
            FolderInput = ConfigurationManager.AppSettings.Get("folder_a");
            FolderOutput = ConfigurationManager.AppSettings.Get("folder_b");
            FolderMeta = ConfigurationManager.AppSettings.Get("folder_c");


            LinesCounter counter = new LinesCounter(FolderMeta);
            FilesCounter filesCounter = new FilesCounter(FolderOutput);

            MidnightTimer midnightTimer = new MidnightTimer(counter);

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


        }
    }
}
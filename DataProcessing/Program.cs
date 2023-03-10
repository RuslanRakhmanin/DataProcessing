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
using System.IO.Pipes;
using Serilog;

namespace DataProcessing // Note: actual namespace depends on the project name.
{
    internal class Program
    {
        static void Main(string[] args)
        {

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File("logs/DataProcessing.log", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            string pipeGUID = "31de4bd9-204b-4c38-9584-32350c750948";
            if (ConfigurationManager.AppSettings.Get("pipeGUID") != null)
            {
                pipeGUID = ConfigurationManager.AppSettings.Get("pipeGUID");
            }


            if (args.Length > 0)
            {
                if (args[0].Equals("stop") || args[0].Equals("reset"))
                {
                    var client = new NamedPipeClientStream(".", pipeGUID);
                    client.Connect();
                    Log.Debug("Client: connected to server");
                    var writer = new StreamWriter(client);
                    writer.WriteLine(args[0]);
                    writer.Flush();
                    client.Dispose();
                    return;
                }
            }

            string FolderInput;
            string FolderOutput;
            string FolderMeta;

            if (ConfigurationManager.AppSettings.Get("folder_a") == null)
            {
                Log.Error("Folder A is not found in the configuration file.");
                return;
            }
            if (ConfigurationManager.AppSettings.Get("folder_b") == null)
            {
                Log.Error("Folder B is not found in the configuration file.");
                return;
            }
            if (ConfigurationManager.AppSettings.Get("folder_c") == null)
            {
                Log.Error("Folder C is not found in the configuration file.");
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




            var server = new NamedPipeServerStream(pipeGUID);

            while (true)
            {
                server.WaitForConnection();

                var reader = new StreamReader(server);
                string? message = reader.ReadLine();
                
                if (message != null && message.Equals("reset"))
                {
                    counter.Reset();
                    txtProcessing.Dispose();
                    csvProcessing.Dispose();
                    txtProcessing = new FilesProcessing(FolderInput, "*.txt", FolderOutput, txtFileProcessor, counter, filesCounter);
                    Task.Run(() => txtProcessing.StartProcessingFileChanges());
                    csvProcessing = new FilesProcessing(FolderInput, "*.csv", FolderOutput, csvFileProcessor, counter, filesCounter);
                    Task.Run(() => csvProcessing.StartProcessingFileChanges());
                }
                else if (message != null && message.Equals("stop"))
                {
                    server.Disconnect();
                    break;
                }

                server.Disconnect();
            }

            txtProcessing.Dispose();
            csvProcessing.Dispose();

            Log.Debug(counter.ToString());


        }
    }
}
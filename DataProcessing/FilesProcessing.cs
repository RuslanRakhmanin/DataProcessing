using System;
using System.Collections.Concurrent;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataProcessing.Interfaces;

namespace DataProcessing
{

    public class FilesProcessing: IDisposable
    {
        private FileSystemWatcher watcher;
        private BlockingCollection<string> queue;
        private ConcurrentDictionary<string, DateTime> processedFileMap;
        IProcessFile fileProcessor;
        string inputFolder;
        string filter;
        string outputFolder;
        LinesCounter counter;
        FilesCounter filesCounter;
        CancellationTokenSource s_cts;

        public FilesProcessing(string inputFolder, string filter, string outputFolder, IProcessFile fileProcessor, LinesCounter counter, FilesCounter filesCounter)
        {
            watcher = new FileSystemWatcher(inputFolder, filter: filter);
            queue = new BlockingCollection<string>();
            processedFileMap = new ConcurrentDictionary<string, DateTime>();
            this.fileProcessor = fileProcessor;
            this.inputFolder = inputFolder;
            this.outputFolder = outputFolder;
            this.counter = counter;
            this.filter = filter;
            this.filesCounter = filesCounter;
            s_cts = new CancellationTokenSource();

            watcher.Changed += (_, e) => queue.Add(e.FullPath);
        }

        
        public void StartProcessingFileChanges()
        {
            string outFolder;
            int fileNumber;
            int linesParsed = 0;
            int linesError = 0;

            //Adding files that exist in the folder
            Directory.GetFiles(inputFolder, filter).ToList().ForEach( x => queue.Add(x));

            //Start watcher
            watcher.EnableRaisingEvents = true;

            //Start consuming queue
            while (!queue.IsCompleted)
            {
                var filePath = queue.Take(); //Blocking dequeue
                var fileInfo = new FileInfo(filePath);

                if (!fileInfo.Exists)
                    continue;

                if (processedFileMap.TryGetValue(filePath, out DateTime processedWithModDate))
                {
                    continue;
                }
                else
                {
                    //We haven't processed this file before. Process it, then save the mod date.
                    processedFileMap.TryAdd(filePath, fileInfo.LastWriteTimeUtc);

                    string fileNameOut = filesCounter.GetNextFileName();
                    var file = File.Create(fileNameOut);
                    file.Close();
                    file.Dispose();

                    Task.Run(() => fileProcessor.ProcessFile(filePath, fileNameOut, this, s_cts.Token), s_cts.Token);

                }
            }
        }

        public void OnFileProcessingEnd(int linesParsed, int linesError, string filePath)
        {

            counter.Add(linesParsed, linesError, filePath);

            processedFileMap.Remove(filePath, out DateTime processedWithModDate);

        }

        public void Dispose()
        {
            watcher.Dispose();
            queue.Dispose();
            s_cts.Cancel();
        }
    }
}

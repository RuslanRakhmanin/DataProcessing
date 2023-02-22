using DataProcessing.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataProcessing
{
    internal class ProcessFileCSV: ProcessFileTXT, IProcessFile
    {
        public ProcessFileCSV() { }

        override public (IEnumerable<string>, bool) GetLinesFromFile(string inputFileName)
        {
            while (!IsFileReady(inputFileName) && File.Exists(inputFileName))
            {

            }

            if (!File.Exists(inputFileName))
            {
                return (Enumerable.Empty<string>(), true);
            }

            try
            {
                IEnumerable<string> inputText = System.IO.File.ReadLines(inputFileName);
                inputText = inputText.Skip(1);
                return (inputText, false);
            }
            catch (IOException e)
            {
                return (Enumerable.Empty<string>(), true);
            }

        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DataProcessing
{
    public class LinesCounter
    {
        int LinesParsed { get; set; }
        int LinesError { get; set; }
        int FilesParsed { get; set; }
        List<string> FilesWithErrors { get;  }
        Object _lock { get; set; }

        public LinesCounter(int filesParsed, int linesParsed, int linesError, List<string> filesWithErrors)
        {
            LinesParsed = linesParsed;
            LinesError = linesError;
            FilesWithErrors = filesWithErrors;
            FilesParsed = filesParsed;
            _lock = new Object();
        }
        public LinesCounter(int filesParsed, int linesParsed, int linesError)
        {
            LinesParsed = linesParsed;
            LinesError = linesError;
            FilesParsed = filesParsed;
            FilesWithErrors = new List<string>();
            _lock = new Object();
        }

        public LinesCounter()
        {
            LinesParsed = 0;
            LinesError = 0;
            FilesParsed = 0;
            FilesWithErrors = new List<string>();
            _lock = new Object();
        }

        public void Add(int linesParsed, int linesError, string fileName)
        {
            lock (_lock)
            {
                LinesParsed += linesParsed;
                LinesError += linesError;
                FilesParsed++;
                if (linesError > 0)
                {
                    FilesWithErrors.Add(fileName);
                }
            }
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(string.Format($"parsed_files: {FilesParsed}"));
            stringBuilder.AppendLine(string.Format($"parsed_lines: {LinesParsed}"));
            stringBuilder.AppendLine(string.Format($"found_errors: {LinesError}"));
            stringBuilder.AppendLine(string.Format($"invalid_files: {JsonSerializer.Serialize(FilesWithErrors)}"));


            return stringBuilder.ToString();

        }
    }
}

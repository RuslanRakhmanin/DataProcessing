using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataProcessing
{
    public class FilesCounter
    {
        int _fileNumber { get; set; }
        string _outFolder { get; set; }

        Object _lock { get; set; }

        public FilesCounter(string outFolder)
        {
            _outFolder = outFolder;
            _fileNumber = 0;
            _lock = new object();
        }

        public string GetNextFileName()
        {

            string outDateFolder = Path.Combine(_outFolder, DateTime.Now.ToString("MM-dd-yyyy"));
            lock (_lock)
            {
                if (!Directory.Exists(outDateFolder))
                {
                    Directory.CreateDirectory(outDateFolder);
                    _fileNumber = 0;
                }
                else
                {
                    
                    _fileNumber++;

                    while (File.Exists(Path.Combine(outDateFolder, string.Format("output{0}.json", _fileNumber))))
                    {
                        _fileNumber++;
                    }
                }
                var fileName = Path.Combine(outDateFolder, string.Format("output{0}.json", _fileNumber));
                return fileName;
            }
        }
    }
}

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

        public int GetNextFileNumber() {
            
            string outDateFolder = Path.Combine(_outFolder, DateTime.Now.ToString("MM-dd-yyyy"));
            if (!Directory.Exists(_outFolder))
            {
                Directory.CreateDirectory(_outFolder);
                _fileNumber = 0;
            }
            else
            {
                //_fileNumber = Directory.GetFiles(outDateFolder, "output*.json").ToList().Count;
                while (File.Exists(Path.Combine(_outFolder, string.Format("output{0}.json", _fileNumber + 1))))
                {
                    _fileNumber++;
                }
            }
            return _fileNumber;
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

                    while (File.Exists(Path.Combine(outDateFolder, string.Format("output{0}.json", _fileNumber + 1))))
                    {
                        _fileNumber++;
                    }
                }
                var fileName = Path.Combine(outDateFolder, string.Format("output{0}.json", _fileNumber + 1));
                return fileName;
            }
        }
    }
}

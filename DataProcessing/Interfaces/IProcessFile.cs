using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataProcessing.Interfaces
{
    public interface IProcessFile
    {
        void ProcessFile(string inputFileName, string outFileName, FilesProcessing FileProcessingObject, CancellationToken cancellationToken);
    }
}

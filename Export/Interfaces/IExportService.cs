using System.IO;
using System.Collections.Generic;

namespace Export.Interfaces
{
    public interface IExportService
    {
        IList<Stream> DownloadFromFileSystem(IList<string> names, string folder, string connectionString);
    }
}
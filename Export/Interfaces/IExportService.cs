using System.IO;
using System.Collections.Generic;

namespace Export.Interfaces
{
    public interface IExportService
    {
        IList<Stream> DownloadFromFileSystem(IList<string> name, string folder, string connectionString);
    }
}
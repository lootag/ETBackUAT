using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Export.Interfaces;

namespace Export
{
    public class ExportService : IExportService
    {
        public IList<Stream> DownloadFromFileSystem(IList<string> names, string folder, string connectionString)
        {
            if (names == null) throw new ArgumentNullException(nameof(names));
            if (folder == null) throw new ArgumentNullException(nameof(folder));
            if (connectionString == null) throw new ArgumentNullException(nameof(connectionString));

            IList<string> paths = names.Select(name => Path.Combine(folder, name)).ToList();
            IList<Stream> toReturn = GetStreamList(paths);
            CloudStorageAccount account = CloudStorageAccount.Parse(connectionString);
            CloudBlobClient blobClient = account.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference("filings");
            IList<CloudBlockBlob> blobs = GetBlobList(names, container);
            DownloadBlobList(blobs, toReturn);
            return toReturn; 
        }

        private IList<Stream> GetStreamList(IList<string> paths)
        {
            IList<Stream> toReturn = new List<Stream>();
            foreach(string path in paths)
            {
                Stream fs = new FileStream(path, FileMode.Create);
                toReturn.Add(fs);
            }
            return toReturn;

        }

        private IList<CloudBlockBlob> GetBlobList(IList<string> names, CloudBlobContainer container)
        {
            IList<CloudBlockBlob> toReturn = new List<CloudBlockBlob>();
            foreach(string name in names)
            {
                try
                {
                    CloudBlockBlob blob = container.GetBlockBlobReference(name);
                    toReturn.Add(blob);
                    
                }
                catch
                {
                    CloudBlockBlob blob = null;
                    toReturn.Add(blob);
                }
                
            }
            return toReturn;
        }

        private void DownloadBlobList(IList<CloudBlockBlob> blobs, IList<Stream> streams)
        {
            for(int i = 0; i < blobs.Count; i++)
            {
                if(blobs[i] != null)
                {
                    blobs[i].DownloadToStream(streams[i]);
                }
                else
                {
                    streams[i] = null;
                }
                
            }
        }
    }
}

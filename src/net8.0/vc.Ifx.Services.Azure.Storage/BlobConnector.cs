using Microsoft.Extensions.Logging;

namespace vc.Ifx.Services.Azure.Storage
{
    public class BlobConnector(ILogger<BlobConnector> logger) : IBlobConnector
    {
        public void UploadFile(string containerName, string blobName, string filePath, OverwriteFile overwriteFile)
        {
        }

        public void DownloadFile(string containerName, string blobName, string downloadFilePath, OverwriteFile overwriteFile)
        {
        }

        public async Task UploadFileAsync(string containerName, string blobName, string filePath, OverwriteFile overwriteFile)
        {
        }

        public async Task DownloadFileAsync(string containerName, string blobName, string downloadFilePath, OverwriteFile overwriteFile)
        {
        }

        public enum OverwriteFile
        {
            No = 0,
            Yes = 1
        }
    }
}

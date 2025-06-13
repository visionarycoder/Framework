namespace v8.Ifx.Services.Cloud.Azure.Storage;

public interface IBlobConnector
{

    void UploadFile(string containerName, string blobName, string filePath, BlobConnector.OverwriteFile overwriteFile);
    void DownloadFile(string containerName, string blobName, string downloadFilePath, BlobConnector.OverwriteFile overwriteFile);

    Task UploadFileAsync(string containerName, string blobName, string filePath, BlobConnector.OverwriteFile overwriteFile);
    Task DownloadFileAsync(string containerName, string blobName, string downloadFilePath, BlobConnector.OverwriteFile overwriteFile);

}
namespace vc.Ifx.Services.Files;

/// <summary>
/// Defines operations for file system interactions.
/// </summary>
public interface IFileService: IFileExists, IFileReadAllText, IFileWriteAllText, IFileDelete, IFileEmpty
{

}
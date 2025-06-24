namespace vc.Ifx.Services.Clipboard;

/// <summary>
/// Defines methods for interacting with the system clipboard.
/// </summary>
public interface IClipboardService : IService
{
    /// <summary>
    /// Determines if this clipboard service can handle the current operating system.
    /// </summary>
    bool CanHandle();

    /// <summary>
    /// Sets the text content of the clipboard.
    /// </summary>
    /// <param name="text">The text to set on the clipboard.</param>
    void SetText(string text);

    /// <summary>
    /// Gets the text content from the clipboard.
    /// </summary>
    /// <returns></returns>
    string GetText();

}
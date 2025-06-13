using System;
using System.Threading.Tasks;

namespace vc.Ifx.Services.Clipboard;

/// <summary>
/// Provides a platform-agnostic interface for clipboard operations.
/// </summary>
public interface IClipboardHelper : IService
{
    /// <summary>
    /// Sets text to the clipboard using the appropriate platform-specific implementation.
    /// </summary>
    /// <param name="text">The text to set to the clipboard.</param>
    /// <exception cref="NotSupportedException">Thrown when no suitable clipboard service is available.</exception>
    void CopyToClipboard(string text);

    /// <summary>
    /// Gets text from the clipboard using the appropriate platform-specific implementation.
    /// </summary>
    /// <returns>The text from the clipboard, or an empty string if the clipboard doesn't contain text.</returns>
    /// <exception cref="NotSupportedException">Thrown when no suitable clipboard service is available.</exception>
    string GetTextFromClipboard();
    
    /// <summary>
    /// Asynchronously sets text to the clipboard.
    /// </summary>
    /// <param name="text">The text to set to the clipboard.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="NotSupportedException">Thrown when no suitable clipboard service is available.</exception>
    Task CopyToClipboardAsync(string text);
    
    /// <summary>
    /// Asynchronously gets text from the clipboard.
    /// </summary>
    /// <returns>A task representing the asynchronous operation with the text from the clipboard.</returns>
    /// <exception cref="NotSupportedException">Thrown when no suitable clipboard service is available.</exception>
    Task<string> GetTextFromClipboardAsync();
    
    /// <summary>
    /// Clears the contents of the clipboard.
    /// </summary>
    /// <exception cref="NotSupportedException">Thrown when no suitable clipboard service is available.</exception>
    void ClearClipboard();
    
    /// <summary>
    /// Checks if the clipboard contains text.
    /// </summary>
    /// <returns>True if the clipboard contains text; otherwise, false.</returns>
    bool ContainsText();
    
    /// <summary>
    /// Attempts to get text from the clipboard.
    /// </summary>
    /// <param name="text">When this method returns, contains the text from the clipboard if successful; otherwise, an empty string.</param>
    /// <returns>True if text was successfully retrieved from the clipboard; otherwise, false.</returns>
    bool TryGetText(out string text);
    
    /// <summary>
    /// Monitors the clipboard for changes.
    /// </summary>
    /// <param name="onChange">The action to perform when the clipboard content changes.</param>
    /// <returns>An IDisposable that can be used to stop monitoring.</returns>
    /// <remarks>This functionality may not be supported on all platforms.</remarks>
    IDisposable MonitorClipboard(Action onChange);
    
    /// <summary>
    /// Gets information about the clipboard capabilities on the current platform.
    /// </summary>
    /// <returns>A ClipboardCapabilities object containing information about supported clipboard features.</returns>
    ClipboardCapabilities GetCapabilities();
}
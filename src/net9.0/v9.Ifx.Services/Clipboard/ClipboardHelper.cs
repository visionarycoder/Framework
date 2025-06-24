using System.Diagnostics;
using System.Threading;

// ReSharper disable ClassNeverInstantiated.Global

namespace vc.Ifx.Services.Clipboard;

/// <summary>
/// Provides helper methods for clipboard operations.
/// </summary>
public class ClipboardHelper(IEnumerable<IClipboardService> clipboardServices) : ServiceBase, IClipboardHelper
{

    /// <summary>
    /// Sets text to the clipboard using the appropriate platform-specific implementation.
    /// </summary>
    /// <param name="text">The text to set to the clipboard.</param>
    /// <exception cref="NotSupportedException">Thrown when no suitable clipboard service is available.</exception>
    public void CopyToClipboard(string text)
    {
        var service = GetSuitableService();
        service.SetText(text);
    }

    /// <summary>
    /// Gets text from the clipboard using the appropriate platform-specific implementation.
    /// </summary>
    /// <returns>The text from the clipboard, or an empty string if the clipboard doesn't contain text.</returns>
    /// <exception cref="NotSupportedException">Thrown when no suitable clipboard service is available.</exception>
    public string GetTextFromClipboard()
    {
        var service = GetSuitableService();
        return service.GetText();
    }

    /// <summary>
    /// Asynchronously sets text to the clipboard.
    /// </summary>
    /// <param name="text">The text to set to the clipboard.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="NotSupportedException">Thrown when no suitable clipboard service is available.</exception>
    public async Task CopyToClipboardAsync(string text)
    {
        await Task.Run(() => CopyToClipboard(text)).ConfigureAwait(false);
    }

    /// <summary>
    /// Asynchronously gets text from the clipboard.
    /// </summary>
    /// <returns>A task representing the asynchronous operation with the text from the clipboard.</returns>
    /// <exception cref="NotSupportedException">Thrown when no suitable clipboard service is available.</exception>
    public async Task<string> GetTextFromClipboardAsync()
    {
        return await Task.Run(GetTextFromClipboard).ConfigureAwait(false);
    }

    /// <summary>
    /// Clears the contents of the clipboard.
    /// </summary>
    /// <exception cref="NotSupportedException">Thrown when no suitable clipboard service is available.</exception>
    public void ClearClipboard()
    {
        CopyToClipboard(string.Empty);
    }

    /// <summary>
    /// Checks if the clipboard contains text.
    /// </summary>
    /// <returns>True if the clipboard contains text; otherwise, false.</returns>
    public bool ContainsText()
    {
        try
        {
            var text = GetTextFromClipboard();
            return !string.IsNullOrEmpty(text);
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Attempts to get text from the clipboard.
    /// </summary>
    /// <param name="text">When this method returns, contains the text from the clipboard if successful; otherwise, an empty string.</param>
    /// <returns>True if text was successfully retrieved from the clipboard; otherwise, false.</returns>
    public bool TryGetText(out string text)
    {
        try
        {
            // Find a suitable service
            foreach (var clipboardService in clipboardServices)
            {
                if (!clipboardService.CanHandle())
                {
                    continue;
                }
                text = clipboardService.GetText();
                return true;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine("An error occurred while trying to get text from the clipboard: " + ex.Message);
        }
        text = string.Empty;
        return false;
    }

    /// <summary>
    /// Monitors the clipboard for changes.
    /// </summary>
    /// <param name="onChange">The action to perform when the clipboard content changes.</param>
    /// <returns>An IDisposable that can be used to stop monitoring.</returns>
    /// <remarks>This functionality may not be supported on all platforms.</remarks>
    /// <exception cref="NotSupportedException">Thrown when clipboard monitoring is not supported on the current platform.</exception>
    public IDisposable MonitorClipboard(Action onChange)
    {
        var capabilities = GetCapabilities();
        if (!capabilities.SupportsMonitoring)
        {
            throw new NotSupportedException("Clipboard monitoring is not supported on the current platform.");
        }

        // Create a monitor that checks for clipboard changes on a timer
        return new ClipboardMonitor(this, onChange);
    }

    /// <summary>
    /// Gets information about the clipboard capabilities on the current platform.
    /// </summary>
    /// <returns>A ClipboardCapabilities object containing information about supported clipboard features.</returns>
    public ClipboardCapabilities GetCapabilities()
    {

        // ReSharper disable ConditionIsAlwaysTrueOrFalse
        // ReSharper disable RedundantAssignment
        var supportsText = false;
        var supportsClear = false;
        var supportsMonitoring = false;

        try
        {
            _ = GetSuitableService(throwIfNotFound: false);
            supportsText = true; // Basic text operations are supported if we have a service
            supportsClear = true; // Clearing is supported if text operations are supported
            supportsMonitoring = false; // Monitoring is generally not supported in a cross-platform manner. Platform-specific implementations could override this.
        }
        catch
        {
            // If we can't get a service, no capabilities are supported
        }

        return new ClipboardCapabilities
        (
            SupportsText: supportsText,
            SupportsAsync: true, // Async is always supported through Task.Run
            SupportsMonitoring: supportsMonitoring,
            SupportsClear: supportsClear
        );
        // ReSharper restore RedundantAssignment
        // ReSharper restore ConditionIsAlwaysTrueOrFalse

    }


    /// <summary>
    /// Gets a suitable clipboard service for the current platform.
    /// </summary>
    /// <param name="throwIfNotFound">Whether to throw an exception if no suitable service is found.</param>
    /// <returns>A clipboard service suitable for the current platform.</returns>
    /// <exception cref="NotSupportedException">Thrown when no suitable clipboard service is available and <paramref name="throwIfNotFound"/> is true.</exception>
    private IClipboardService GetSuitableService(bool throwIfNotFound = true)
    {
        foreach (var clipboardService in clipboardServices)
        {
            if (clipboardService.CanHandle())
            {
                return clipboardService;
            }
        }
        if (throwIfNotFound)
        {
            throw new NotSupportedException("No clipboard service available for the current platform.");
        }
        return null;
    }

    /// <summary>
    /// A disposable class that monitors clipboard changes.
    /// </summary>
    private class ClipboardMonitor : IDisposable
    {

        private readonly Timer timer;
        private bool disposed;

        public ClipboardMonitor(ClipboardHelper helper, Action onChange)
        {
            var previousText = helper.TryGetText(out var text) ? text : string.Empty;

            // Check for changes every 500ms
            timer = new Timer(_ =>
            {
                if (disposed) 
                    return;

                if (!helper.TryGetText(out var currentText) || currentText == previousText) return;
                previousText = currentText;
                onChange?.Invoke();
            }, null, 0, 500);
        }

        public void Dispose()
        {
            if (!disposed)
            {
                timer?.Dispose();
                disposed = true;
            }
        }
    }
}
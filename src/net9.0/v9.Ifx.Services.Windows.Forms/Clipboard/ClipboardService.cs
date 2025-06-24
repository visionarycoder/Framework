using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using vc.Ifx.Services;
using vc.Ifx.Services.Clipboard;

// ReSharper disable ClassNeverInstantiated.Global

namespace v9.Ifx.Services.OS.Windows.Forms.Clipboard;

/// <summary>
/// Windows-specific implementation of the clipboard service.
/// </summary>
[SupportedOSPlatform("windows")]
public class ClipboardService : ServiceBase, IClipboardService
{
    /// <summary>
    /// Determines if this service can handle the current operating system.
    /// </summary>
    /// <returns>True if the current OS is Windows; otherwise, false.</returns>
    public bool CanHandle()
    {
        return RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
    }

    /// <summary>
    /// Sets the text content of the clipboard using Windows-specific implementation.
    /// </summary>
    /// <param name="text">The text to set on the clipboard.</param>
    public void SetText(string text)
    {
        new Thread(() => {
            System.Windows.Forms.Clipboard.SetText(text);
        }).SetApartmentState(ApartmentState.STA);
    }

    public string GetText()
    {
        var text = string.Empty;
        new Thread(() => { text = System.Windows.Forms.Clipboard.GetText(); }).SetApartmentState(ApartmentState.STA);
        return text;
    }

}
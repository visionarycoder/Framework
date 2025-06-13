using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using vc.Ifx.Services;
using vc.Ifx.Services.Clipboard;



namespace v9.Ifx.Services.OS.Mac.Clipboard;

/// <summary>
/// macOS-specific implementation of the clipboard service.
/// </summary>
[SupportedOSPlatform("macos")]
public class ClipboardService : ServiceBase, IClipboardService
{
    /// <summary>
    /// Determines if this service can handle the current operating system.
    /// </summary>
    /// <returns>True if the current OS is macOS; otherwise, false.</returns>
    public bool CanHandle()
    {
        return RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
    }

    /// <summary>
    /// Sets the text content of the clipboard using macOS-specific implementation.
    /// </summary>
    /// <param name="text">The text to set on the clipboard.</param>
    public void SetText(string text)
    {
        using var process = new Process();
        process.StartInfo = new ProcessStartInfo
        {
            FileName = "pbcopy",
            UseShellExecute = false,
            RedirectStandardInput = true
        };
        process.Start();
        process.StandardInput.Write(text);
        process.StandardInput.Close();
        process.WaitForExit();
    }

    /// <summary>
    /// Gets the text content from the clipboard using macOS-specific implementation.
    /// </summary>
    /// <returns>The text from the clipboard or empty string if operation fails.</returns>
    public string GetText()
    {
        try
        {
            using var process = new Process();
            process.StartInfo = new ProcessStartInfo
            {
                FileName = "pbpaste",
                UseShellExecute = false,
                RedirectStandardOutput = true
            };
            process.Start();
            var result = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            return result;
        }
        catch (Exception)
        {
            // If clipboard operation fails, return empty string
            return string.Empty;
        }
    }
}
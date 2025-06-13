using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using vc.Ifx.Services;
using vc.Ifx.Services.Clipboard;

// ReSharper disable ClassNeverInstantiated.Global

namespace v9.Ifx.Services.OS.Linux.Clipboard;

/// <summary>
/// Linux-specific implementation of the clipboard service.
/// </summary>
[SupportedOSPlatform("linux")]
public class ClipboardService : ServiceBase, IClipboardService
{
    /// <summary>
    /// Determines if this service can handle the current operating system.
    /// </summary>
    /// <returns>True if the current OS is Linux; otherwise, false.</returns>
    public bool CanHandle()
    {
        return RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
    }

    /// <summary>
    /// Sets the text content of the clipboard using Linux-specific implementation.
    /// </summary>
    /// <param name="text">The text to set on the clipboard.</param>
    public void SetText(string text)
    {
        try
        {
            // Try with xclip first
            using var process = new Process();
            process.StartInfo = new ProcessStartInfo
            {
                FileName = "xclip",
                Arguments = "-selection clipboard",
                UseShellExecute = false,
                RedirectStandardInput = true
            };
            process.Start();
            process.StandardInput.Write(text);
            process.StandardInput.Close();
            process.WaitForExit();
        }
        catch (Win32Exception)
        {
            // Try with xsel if xclip is not available  
            using var process = new Process();
            process.StartInfo = new ProcessStartInfo
            {
                FileName = "xsel",
                Arguments = "--clipboard --input",
                UseShellExecute = false,
                RedirectStandardInput = true
            };
            process.Start();
            process.StandardInput.Write(text);
            process.StandardInput.Close();
            process.WaitForExit();
        }
    }

    public string GetText()
    {
        try
        {
            // Try with xclip first
            using var process = new Process();
            process.StartInfo = new ProcessStartInfo
            {
                FileName = "xclip",
                Arguments = "-selection clipboard -o",
                UseShellExecute = false,
                RedirectStandardOutput = true
            };
            process.Start();
            var result = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            return result;
        }
        catch (Win32Exception)
        {
            try
            {
                // Try with xsel if xclip is not available
                using var process = new Process();
                process.StartInfo = new ProcessStartInfo
                {
                    FileName = "xsel",
                    Arguments = "--clipboard --output",
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
                // If all clipboard methods fail, return empty string
                return string.Empty;
            }
        }
    }
}
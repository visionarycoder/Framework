namespace vc.Ifx.Services.Clipboard;

/// <summary>
/// Represents the clipboard capabilities available on the current platform.
/// </summary>
public record ClipboardCapabilities(bool SupportsText, bool SupportsAsync, bool SupportsMonitoring, bool SupportsClear);
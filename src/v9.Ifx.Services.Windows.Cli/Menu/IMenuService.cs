namespace v9.Ifx.Services.OS.Windows.Cli.Menu;

public interface IMenuService
{
    int PromptForInteger(string promptText, int? defaultValue = null, Func<int, bool>? validator = null);
    string PromptForString(string promptText, string? defaultValue = null, Func<string, bool>? validator = null);
    bool PromptForYesNo(string promptText, bool? defaultValue = null);
    string PromptForChoice(string promptText, IEnumerable<string> choices, string? defaultValue = null);
}
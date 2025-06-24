using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;

namespace vc.Ifx.CodeGen;

[Generator]
public class MonthGenerator : IIncrementalGenerator
{

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {

    }

    public void Execute(GeneratorExecutionContext context)
    {
        var longNames = new[] { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };
        var shortNames = new[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };

        var sb = new StringBuilder();
        sb.AppendLine("namespace vc.Ifx;");
        sb.AppendLine("public partial class Month");
        sb.AppendLine("{");
        sb.AppendLine("    #region generated");
        sb.AppendLine("    public const string UNKNOWN = \"???\";");
        sb.AppendLine("    public static readonly List<string> LongMonthNames = [UNKNOWN, " + string.Join(", ", longNames.Select(n => n.ToUpperInvariant())) + "];");
        sb.AppendLine("    public static readonly List<string> ShortMonthNames = [UNKNOWN, " + string.Join(", ", shortNames.Select(n => n.ToUpperInvariant())) + "];");

        for (var i = 0; i < 12; i++)
        {
            sb.Append($"    public const string {longNames[i].ToUpperInvariant()} = \"{longNames[i]}\";").AppendLine();
        }
        
        for (var i = 0; i < 12; i++)
        {
            sb.Append($"    public const string {shortNames[i].ToUpperInvariant()} = \"{shortNames[i]}\";").AppendLine();
        }

        sb.AppendLine("    #endregion generated");
        sb.AppendLine("}");

        //   context.AddSource("Month.g.cs", SourceText.From(sb.ToString(), Encoding.UTF8));

    }

}
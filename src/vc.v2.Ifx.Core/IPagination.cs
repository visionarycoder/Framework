namespace vc.v2.Ifx.Core;

public interface IPagination
{
    int Skip { get; set; }
    int? Take { get; set; }
}
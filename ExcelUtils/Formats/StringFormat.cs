using ExcelUtile.ExcelCore;

namespace ExcelUtile.Formats;

internal class StringFormat : ExcelConverter
{
    public StringFormat() : base(typeof(string))
    {
    }

    public override object? ReadFromCell(ICell cell)
    {
        return cell.GetString();
    }
}
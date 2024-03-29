using ExcelUtile.ExcelCore;

namespace ExcelUtile.Formats;

public class DefaultFormat : ExcelConverter
{
    public DefaultFormat() : base(typeof(string))
    {
    }

    public override bool CanConvert(Type type) => true;

    public override object? ReadFromCell(ICell cell)
    {
        return cell.GetString();
    }
}
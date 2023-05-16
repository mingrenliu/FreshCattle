namespace ExcelUtile.Formats;

internal class ShortTimeFormat : DateTimeFormat
{
    public override void Write(ICell cell, DateTime? value)
    {
        if (value != null)
        {
            cell.SetCellValue(value.Value);
        }
    }
}
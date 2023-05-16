namespace ExcelUtile.Formats;

internal class LongTimeFormat : DateTimeFormat
{
    public override void Write(ICell cell, DateTime? value)
    {
        if (value != null)
        {
            cell.SetCellValue(value.Value.ToString("yyyy-MM-dd HH:mm:ss"));
        }
    }
}
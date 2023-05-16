namespace ExcelUtile.Formats;

internal class OnlyTimeFormat : DateTimeFormat
{
    public override void Write(ICell cell, DateTime? value)
    {
        if (value != null)
        {
            cell.SetCellValue(value.Value.ToString("HH:mm:ss"));
        }
    }
}
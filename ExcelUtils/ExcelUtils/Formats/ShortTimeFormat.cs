using NPOI.HSSF.UserModel;

namespace ExcelUtile.Formats;

public class ShortTimeFormat : DateTimeFormat
{
    protected override string? Format => "yyyy-mm-dd";

    protected override void WriteValue(ICell cell, DateTime? value)
    {
        if (value != null)
        {
            cell.SetCellValue(value.Value);
        }
    }
}
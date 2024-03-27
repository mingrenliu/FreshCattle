using ExcelUtile.ExcelCore;

namespace ExcelUtile.Formats;

public class StringFormat : ExcelReferenceConverter<string>
{
    public override string? Read(ICell cell) => cell.GetString();

    public override void Write(ICell cell, string? value)
    {
        if (string.IsNullOrWhiteSpace(value) is false)
        {
            cell.SetCellValue(value);
        }
    }
}
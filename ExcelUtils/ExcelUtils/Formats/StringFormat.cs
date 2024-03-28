using ExcelUtile.ExcelCore;

namespace ExcelUtile.Formats;

public class StringFormat : ExcelReferenceConverter<string>
{
    public override string? Read(ICell cell) => cell.GetString();

    protected override void WriteValue(ICell cell, string? value)
    {
        if (string.IsNullOrWhiteSpace(value) is false)
        {
            cell.SetCellValue(value);
        }
    }
}
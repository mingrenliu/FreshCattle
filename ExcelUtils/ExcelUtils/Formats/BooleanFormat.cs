using ExcelUtile.ExcelCore;

namespace ExcelUtile.Formats;

public class BooleanFormat : ExcelStructConverter<bool>
{
    private readonly string? TrueValue = "是";
    private readonly string? FalseValue = "否";

    public BooleanFormat()
    {
    }

    public BooleanFormat(string? trueValue)
    {
        if (string.IsNullOrWhiteSpace(trueValue) is false)
        {
            TrueValue = trueValue;
        }
    }

    public BooleanFormat(string? trueValue, string? falseValue) : this(trueValue)
    {
        if (string.IsNullOrWhiteSpace(falseValue) is false)
        {
            FalseValue = falseValue;
        }
    }

    public override bool? Read(ICell cell)
    {
        if (CanConvert(typeof(bool)))
        {
            return cell.GetBoolean();
        }
        return null;
    }

    protected override void WriteValue(ICell cell, bool? value)
    {
        if (value.HasValue)
        {
            cell.SetCellValue(value.Value ? TrueValue : FalseValue);//待定
        }
    }
}
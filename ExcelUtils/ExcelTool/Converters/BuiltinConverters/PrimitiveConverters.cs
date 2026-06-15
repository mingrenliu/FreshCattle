using NPOI.SS.UserModel;
using ExcelTool.ExcelCore;

namespace ExcelTool.Converters;

public class StringConverter : ExcelConverter<string>
{
    public override string Read(ICell cell) => cell.GetString() ?? string.Empty;

    public override void Write(ICell cell, string value, ICellStyle? style = null)
    {
        cell.SetCellValue(value);
        if (style != null) cell.CellStyle = style;
    }
}

public class Int32Converter : ExcelConverter<int>
{
    public override int Read(ICell cell) => cell.GetInt() ?? 0;
    public override void Write(ICell cell, int value, ICellStyle? style = null)
    {
        cell.SetCellValue((double)value);
        if (style != null) cell.CellStyle = style;
    }
}

public class Int64Converter : ExcelConverter<long>
{
    public override long Read(ICell cell) => cell.GetLong() ?? 0L;
    public override void Write(ICell cell, long value, ICellStyle? style = null)
    {
        cell.SetCellValue(value);
        if (style != null) cell.CellStyle = style;
    }
}

public class Int16Converter : ExcelConverter<short>
{
    public override short Read(ICell cell) => cell.GetShort() ?? 0;
    public override void Write(ICell cell, short value, ICellStyle? style = null)
    {
        cell.SetCellValue((double)value);
        if (style != null) cell.CellStyle = style;
    }
}

public class ByteConverter : ExcelConverter<byte>
{
    public override byte Read(ICell cell)
    {
        var v = cell.GetInt();
        return (v.HasValue && v >= 0 && v <= 255) ? (byte)v.Value : (byte)0;
    }
    public override void Write(ICell cell, byte value, ICellStyle? style = null)
    {
        cell.SetCellValue((double)value);
        if (style != null) cell.CellStyle = style;
    }
}

public class DoubleConverter : ExcelConverter<double>
{
    public DoubleConverter() { }
    public DoubleConverter(string? format) { ExcelFormat = format; }

    public override double Read(ICell cell) => cell.GetDouble() ?? 0d;
    public override void Write(ICell cell, double value, ICellStyle? style = null)
    {
        cell.SetCellValue(value);
        if (style != null) cell.CellStyle = style;
    }
}

public class SingleConverter : ExcelConverter<float>
{
    public override float Read(ICell cell) => cell.GetFloat() ?? 0f;
    public override void Write(ICell cell, float value, ICellStyle? style = null)
    {
        cell.SetCellValue((double)value);
        if (style != null) cell.CellStyle = style;
    }
}

public class DecimalConverter : ExcelConverter<decimal>
{
    public DecimalConverter() { }
    public DecimalConverter(string? format) { ExcelFormat = format; }

    public override decimal Read(ICell cell) => cell.GetDecimal() ?? 0m;
    public override void Write(ICell cell, decimal value, ICellStyle? style = null)
    {
        cell.SetCellValue((double)value);
        if (style != null) cell.CellStyle = style;
    }
}

public class BooleanConverter : ExcelConverter<bool>
{
    private readonly string _trueValue;
    private readonly string _falseValue;

    public BooleanConverter(string trueValue = "是", string falseValue = "否")
    {
        _trueValue = trueValue;
        _falseValue = falseValue;
    }

    public override bool Read(ICell cell)
    {
        var b = cell.GetBoolean();
        if (b.HasValue) return b.Value;
        var s = cell.GetString();
        if (string.IsNullOrWhiteSpace(s)) return false;
        if (string.Equals(s, _trueValue, StringComparison.OrdinalIgnoreCase)) return true;
        if (string.Equals(s, _falseValue, StringComparison.OrdinalIgnoreCase)) return false;
        if (string.Equals(s, "true", StringComparison.OrdinalIgnoreCase)) return true;
        if (string.Equals(s, "false", StringComparison.OrdinalIgnoreCase)) return false;
        return false;
    }

    public override void Write(ICell cell, bool value, ICellStyle? style = null)
    {
        cell.SetCellValue(value ? _trueValue : _falseValue);
        if (style != null) cell.CellStyle = style;
    }
}

/// <summary>
/// 兜底转换器：对 object 类型，Write 时按运行时类型委托给对应转换器，Read 时返回 NPOI 原始值。
/// </summary>
public class ObjectConverter : ExcelConverter<object>
{
    public override object Read(ICell cell)
    {
        return cell.GetObject() ?? string.Empty;
    }

    public override void Write(ICell cell, object value, ICellStyle? style = null)
    {
        if (value == null) return;

        var type = Nullable.GetUnderlyingType(value.GetType()) ?? value.GetType();
        if (type == typeof(object) || type == typeof(string))
        {
            cell.SetCellValue(value.ToString());
        }
        else if (BuiltinConverters.TryGetConverter(type, out var builtin))
        {
            builtin.WriteObject(cell, value, style);
        }
        else
        {
            cell.SetCellValue(value.ToString());
        }
        if (style != null) cell.CellStyle = style;
    }
}

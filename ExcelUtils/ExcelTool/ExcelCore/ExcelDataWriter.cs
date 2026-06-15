using ExcelTool.Converters;
using NPOI.SS.UserModel;

namespace ExcelTool.ExcelCore;

/// <summary>
/// 数组对象写入器，将 <c>IEnumerable&lt;T&gt;</c> 按属性映射写入 Sheet。
/// 继承自 <see cref="ExcelSheetWriter"/>。
/// 仅依赖 <see cref="ExcelWriterTypeInfo"/>：ColumnName 用于表头，Order/Width 用于布局。
/// </summary>
public class ExcelDataWriter<T> : ExcelSheetWriter where T : class
{
    public readonly ExcelOptions Options;
    public readonly ExcelWriterTypeInfo TypeInfo;
    private readonly ExcelConverter _defaultConverter = BuiltinConverters.Fallback;
    protected override float DefaultRowHeight => Options.DefaultRowHeight;
    public ExcelDataWriter(ISheet sheet, ExcelWriterTypeInfo typeInfo, ExcelOptions options) : base(sheet)
    {
        TypeInfo = typeInfo;
        Options = options;
    }

    public void Write(IEnumerable<T> data)
    {
        MoveTo(Options.EffectiveDataStartRow, 0);
        foreach (var item in data)
        {
            if (item == null) continue;
            WriteRow(item);
            NextRow();
        }
        WriteHeader();
        Merge(Options.MergedRegions);
    }

    public IRow WriteHeader()
    {
        MoveTo(Options.HeaderRow, 0);
        foreach (var prop in TypeInfo.Columns)
        {
            Write(prop.ColumnName, _defaultConverter, Style("__ExcelHeader__"));
            PrevWidth(prop.Width != 0 ? prop.Width : Options.DefaultColumnWidth);
        }
        return CurrentRow();
    }

    public void WriteRows(IEnumerable<T> data)
    {
        foreach (var item in data)
        {
            if (item == null) continue;
            WriteRow(item);
            NextRow();
        }
    }

    public IRow WriteRow(T item)
    {
        foreach (var prop in TypeInfo.Columns)
        {
            var value = prop.GetValue(item);
            var converter = prop.Converter ?? _defaultConverter;
            Write(value, converter, Style(prop.ColumnName));
        }
        return CurrentRow();
    }
}
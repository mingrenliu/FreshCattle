using ExcelUtile.Converters;

namespace ExcelUtile.ExcelCore;

/// <summary>
/// 数组对象写入器，将 <c>IEnumerable&lt;T&gt;</c> 按属性映射写入 Sheet。
/// 建立在 <see cref="ExcelSheetWriter"/> 之上。
/// 仅依赖 <see cref="ExcelWriterTypeInfo"/>：ColumnName 用于表头，Order/Width 用于布局。
/// </summary>
internal class ExcelDataWriter<T> where T : class
{
    private readonly ExcelSheetWriter _writer;
    private readonly ExcelOptions _options;
    private readonly ExcelWriterTypeInfo _typeInfo;
    private readonly ExcelConverter _defaultConverter;

    public ExcelDataWriter(ExcelSheetWriter writer, ExcelWriterTypeInfo typeInfo, ExcelOptions options)
    {
        _writer = writer;
        _typeInfo = typeInfo;
        _options = options;
        _defaultConverter = BuiltinConverters.Fallback;
    }

    public void Write(IEnumerable<T> data)
    {
        _writer.MoveTo(_options.EffectiveDataStartRow, 0);
        foreach (var item in data)
        {
            if (item == null) continue;
            WriteRow(item);
            _writer.NextRow();
        }
        WriteHeader();
        _writer.Merge(_options.MergedRegions);
    }

    private IRow WriteHeader()
    {
        _writer.MoveTo(_options.HeaderRow, 0);
        foreach (var prop in _typeInfo.Columns)
        {
            _writer.Write(prop.ColumnName, _defaultConverter,_writer.Style("__ExcelHeader__"));
            _writer.PrevWidth(prop.Width != 0 ? prop.Width : _options.DefaultColumnWidth);
        }
        return _writer.CurrentRow();
    }

    private IRow WriteRow(T item)
    {
        foreach (var prop in _typeInfo.Columns)
        {
            var value = prop.GetValue(item);
            var converter = prop.Converter ?? _defaultConverter;
            _writer.Write(value, converter, _writer.Style(prop.ColumnName));
        }
        return _writer.CurrentRow();
    }
}

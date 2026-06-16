using ExcelTool.Converters;

namespace ExcelTool.ExcelCore;

/// <summary>
/// 数组对象读取器，从 Sheet 按表头匹配属性并反序列化为 <c>T</c>。
/// 继承自 <see cref="ExcelSheetReader"/>。
/// 仅依赖 <see cref="ExcelReaderTypeInfo"/>：通过 <see cref="IExcelColumnReader.Match"/> 匹配表头，<see cref="IExcelColumnReader.SetValue"/> 写回数据。
/// </summary>
public class ExcelDataReader<T> : ExcelSheetReader where T : class, new()
{
    public readonly ExcelOptions Options;
    public readonly ExcelReaderTypeInfo TypeInfo;
    private readonly ExcelConverter _defaultConverter = BuiltInConverters.Fallback;

    /// <summary>匹配到的列：(列索引, 列名, 列信息)。</summary>
    private List<(int ColIdx, string ColumnName, IExcelColumnReader Reader)>? Columns;

    public ExcelDataReader(ISheet sheet, ExcelReaderTypeInfo typeInfo,
        ExcelOptions options) : base(sheet)
    {
        TypeInfo = typeInfo;
        Options = options;
    }


    public List<T> Read()
    {
        var results=new List<T>();
        if (!InitHeader()) return results;

        for (int rowIdx = Options.EffectiveDataStartRow; rowIdx <= LastRowNum; rowIdx++)
        {
            var obj = ReadRow(rowIdx);
            if (obj != null)
            {
                results.Add(obj);
            }
        }
        return results;
    }

    private bool InitHeader()
    {
        if (LastRowNum < Options.HeaderRow) return false;
        if (Options.EffectiveDataStartRow > LastRowNum) return false;

        var headers = ReadHeaders(Options.HeaderRow);
        if (headers.Count == 0) return false;

        Columns = new List<(int, string, IExcelColumnReader)>();
        var matchedProps = new HashSet<IExcelColumnReader>();

        foreach (var (colIdx, text) in headers)
        {
            foreach (var prop in TypeInfo.Columns)
            {
                if (prop.Match(text))
                {
                    Columns.Add((colIdx, text, prop));
                    matchedProps.Add(prop);
                    break;
                }
            }
        }

        if (Columns.Count == 0) return false;

        if (Options.ValidateRequired)
        {
            foreach (var prop in TypeInfo.Columns.Where(p => p.Required))
            {
                if (!matchedProps.Contains(prop))
                    throw new InvalidOperationException($"导入失败，缺少必填字段列: {prop}");
            }
        }

        return true;
    }

    private T? ReadRow(int rowIndex)
    {
        var obj = new T();

        foreach (var (colIdx, columnName, reader) in Columns!)
        {
            var cell = GetMergedCell(rowIndex, colIdx);
            if (cell == null) continue;

            var converter = reader.Converter ?? _defaultConverter;
            var value = converter.ReadObject(cell);
            reader.SetValue(obj, columnName, value);
        }

        return obj;
    }
}
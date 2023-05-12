namespace ExcelUtils.OldVersion;

internal class ExcelImportHandler<T> where T : class, new()
{
    private readonly Dictionary<string, PropertyInfos> infos;
    private readonly Type type;

    public ExcelImportHandler()
    {
        type = typeof(T);
        infos = type.ParsePropertys();
    }

    internal IEnumerable<T> GetDatas(ISheet sheet)
    {
        var headers = ExcelImportHandler<T>.GetHeaders(sheet);
        Filter(headers);
        Validate(headers);
        for (int row = 1; row <= sheet.LastRowNum; row++)
        {
            yield return GetEntity(sheet.GetRow(row));
        }
        T GetEntity(IRow row)
        {
            T obj = (Activator.CreateInstance(type!) as T) ?? throw new Exception("导入类型无法创建");
            foreach (var colume in headers)
            {
                ICell cell = row.GetCell(colume.Key);
                PropertyInfos info = infos[colume.Value];
                ExcelImportHandler<T>.SetValue(cell, obj, info);
            }
            return obj;
        }
    }

    private static void SetValue(ICell cell, object entity, PropertyInfos info)
    {
        var str=cell.ToString();
        var min = cell.Row.FirstCellNum;
        var max = cell.Row.LastCellNum;
        double height = cell.Row.Sheet.GetColumnWidth(cell.ColumnIndex)/256;
        if (cell.CellType == CellType.Unknown || cell.CellType == CellType.Blank || cell.CellType == CellType.Error)
        {
            return;
        }
        Type? underlyingType = Nullable.GetUnderlyingType(info.Info.PropertyType);
        if (underlyingType == typeof(DateTime) || info.Info.PropertyType == typeof(DateTime))
        {
            if (cell.CellType == CellType.Numeric || cell.CellType == CellType.Formula && cell.CachedFormulaResultType == CellType.Numeric)
            {
                info.Info.SetValue(entity, cell.DateCellValue);
            }
            else
            {
                if (DateTime.TryParse(cell.StringCellValue?.Replace("：", ":"), out var dt))
                {
                    info.Info.SetValue(entity, dt);
                }
            }
        }
        else if (underlyingType == typeof(bool) || info.Info.PropertyType == typeof(bool))
        {
            if (cell.CellType == CellType.Boolean || cell.CellType == CellType.Formula && cell.CachedFormulaResultType == CellType.Boolean)
            {
                info.Info.SetValue(entity, cell.BooleanCellValue);
            }
            else
            {
                string? value = cell.ToString()?.Trim();
                if (!string.IsNullOrEmpty(value))
                {
                    if (value.Equals("是") || value.Equals("true", StringComparison.OrdinalIgnoreCase))
                    {
                        info.Info.SetValue(entity, true);
                    }
                    else
                    {
                        info.Info.SetValue(entity, false);
                    }
                }
            }
        }
        else
        {
            try
            {
                if (cell.CellType == CellType.Formula)
                {
                    if (cell.CachedFormulaResultType == CellType.Numeric)
                    {
                        info.Info.SetValue(entity, Convert.ChangeType(cell.NumericCellValue, underlyingType ?? info.Info.PropertyType));
                    }
                    else if (cell.CachedFormulaResultType == CellType.String)
                    {
                        info.Info.SetValue(entity, Convert.ChangeType(cell.StringCellValue, underlyingType ?? info.Info.PropertyType));
                    }
                }
                else
                {
                    string? value = cell.ToString()?.Trim();
                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        info.Info.SetValue(entity, Convert.ChangeType(value, underlyingType ?? info.Info.PropertyType));
                    }
                }
            }
            catch (Exception)
            {
                //忽略该条数据
            }
        }
    }

    private void Filter(Dictionary<int, string> headers)
    {
        foreach (var pair in headers)
        {
#pragma warning disable CA1853 // Unnecessary call to 'Dictionary.ContainsKey(key)'
            if (!infos.ContainsKey(pair.Value))
            {
                headers.Remove(pair.Key);
            }
#pragma warning restore CA1853 // Unnecessary call to 'Dictionary.ContainsKey(key)'
        }
    }

    private void Validate(Dictionary<int, string> headers)
    {
        var values = headers.Values;
        foreach (var info in infos.Values.Where(x => x.AttributeInfo.IsRequird))
        {
            if (values.Any(x => x == info.Name))
            {
                continue;
            }
            throw new Exception($"导入失败,必须导入字段缺失:{info.Name}");
        }
    }

    private static Dictionary<int, string> GetHeaders(ISheet sheet)
    {
        var results = new Dictionary<int, string>();
        var row = sheet.GetRow(0);
        foreach (var cell in row.Cells)
        {
            var cellStr = cell.StringCellValue;
            if (!string.IsNullOrWhiteSpace(cellStr))
            {
                results[cell.ColumnIndex] = cellStr.Trim();
            }
        }
        return results;
    }
}
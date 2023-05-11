global using NPOI.SS.UserModel;

namespace ExcelUtils.OldVersion;

internal class ExcelExportHandler<T> where T : class, new()
{
    private readonly List<PropertyInfos> infos;
    private readonly Type type;

    public ExcelExportHandler()
    {
        type = typeof(T);
        infos = type.Propertys();
    }

    internal void SetDatas(ISheet sheet, List<T> datas)
    {
        SetHeaders(sheet);
        for (int rowIndex = 1; rowIndex < datas.Count; rowIndex++)
        {
            var row = sheet.CreateRow(rowIndex);
            for (var columeIndex = 0; columeIndex < infos.Count; columeIndex++)
            {
                var cell = row.CreateCell(columeIndex);
                ExcelExportHandler<T>.SetCell(cell, infos[columeIndex], datas[rowIndex]);
            }
        }
    }

    private static void SetCell(ICell cell, PropertyInfos info, object entity)
    {
        cell.SetCellValue(info.Info.GetValue(entity)?.ToString());
    }

    private void SetHeaders(ISheet sheet)
    {
        var row = sheet.CreateRow(0);
        for (var columeIndex = 0; columeIndex < infos.Count; columeIndex++)
        {
            var cell = row.CreateCell(columeIndex);
            cell.SetCellType(CellType.String);
            cell.SetCellValue(infos[columeIndex].Name);
            cell.Row.Sheet.SetColumnWidth(0, 30*256);
        }
    }
}
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelUtile.OldVersion
{
    internal static class CellStyleHelper
    {
        internal static void SetStyle(this ICell cell,string? format)
        {
            if (string.IsNullOrEmpty(format)) return;
            var workbook = cell.Row.Sheet.Workbook;
            var cellstype=workbook.CreateCellStyle();
            var dataformat=workbook.CreateDataFormat();
            cellstype.DataFormat = dataformat.GetFormat(format);
            cell.CellStyle=cellstype;
        }
    }
}

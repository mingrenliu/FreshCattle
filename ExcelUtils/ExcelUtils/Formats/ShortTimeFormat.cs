﻿using NPOI.HSSF.UserModel;

namespace ExcelUtile.Formats;

public class ShortTimeFormat : DateTimeFormat
{
    protected override string? _format => "yyyy-mm-dd";

    public override void Write(ICell cell, DateTime? value)
    {
        if (value != null)
        {
            cell.SetCellValue(value.Value);
        }
    }
}
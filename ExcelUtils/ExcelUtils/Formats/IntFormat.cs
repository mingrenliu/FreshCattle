﻿using ExcelUtile.ExcelCore;

namespace ExcelUtile.Formats;

public class IntFormat : ExcelStructConverter<int>
{
    protected override string? Format => "0";

    public override int? Read(ICell cell)
    {
        if (CanConvert())
        {
            return cell.GetInt();
        }
        return default;
    }

    protected override void WriteValue(ICell cell, int? value)
    {
        if (value.HasValue)
        {
            cell.SetCellValue(value.Value);//待定
        }
    }
}
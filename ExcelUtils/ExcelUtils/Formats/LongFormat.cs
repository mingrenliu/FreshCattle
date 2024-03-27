﻿using ExcelUtile.ExcelCore;

namespace ExcelUtile.Formats;

public class LongFormat : ExcelStructConverter<long>
{
    protected override string? Format => "0";

    public override long? Read(ICell cell)
    {
        if (CanConvert())
        {
            return cell.GetLong();
        }
        return default;
    }

    public override void Write(ICell cell, long? value)
    {
        if (value.HasValue)
        {
            cell.SetCellValue(value.Value);//待定
        }
    }
}